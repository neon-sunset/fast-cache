using System.Buffers;
using System.Diagnostics;
using FastCache.Services;

namespace FastCache;

internal sealed class EvictionQuickList<T> where T : notnull
{
    private (int, long)[] _active;
    private (int, long)[] _inactive;
    private ulong _count;

    public uint Count => (uint)Interlocked.Read(ref _count);

    public EvictionQuickList()
    {
        _active = new (int, long)[Constants.QuickListLength];
        _inactive = new (int, long)[Constants.QuickListLength];
        _count = 0;
    }

    public void Add(int value, long expiresAt)
    {
        var entries = _active;
        var count = Count;
        if (count < entries.Length)
        {
            entries[count] = (value, expiresAt);

            Interlocked.CompareExchange(ref _count, count + 1, count);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void OverwritingNonAtomicAdd(int value, long expiresAt)
    {
        var entries = _active;
        var count = (uint)_count;
        if (count < entries.Length)
        {
            entries[count] = (value, expiresAt);
            _count = count + 1;
        }
    }

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    // Performs cache eviction by iterating through quick list and removing expired entries from cache store.
    // Returns 'true' if resident cache size is contained within quick list, 'false' if full eviction is required
    internal bool Evict(long now)
    {
#if DEBUG
        var sw = Stopwatch.StartNew();
#endif
        var store = Cached<T>.s_store;

        var totalCount = store.Count;
        if (totalCount is 0)
        {
            return true;
        }
        else if (_count is 0)
        {
            return false;
        }

        lock (this)
        {
            var entries = _active;
            var entriesCount = Count;

            var entriesSurvivedIndexes = ArrayPool<uint>.Shared.Rent((int)entriesCount);

            uint entriesRemovedCount = 0;
            uint entriesSurvivedCount = 0;

            for (uint i = 0; i < entriesCount; i++)
            {
                var (identifier, expiresAt) = entries[i];

                if (now > expiresAt)
                {
                    if (store.TryGetValue(identifier, out var inner))
                    {
                        var itemExpiresAt = inner._expiresAt;
                        if (now > itemExpiresAt)
                        {
                            store.TryRemove(identifier, out _);
                            entriesRemovedCount++;
                        }
                        else
                        {
                            entries[i] = (identifier, itemExpiresAt);
                            entriesSurvivedIndexes[entriesSurvivedCount] = i;
                            entriesSurvivedCount++;
                        }
                    }
                    else
                    {
                        // Duplicate entry present in quick list has already been removed from cache store.
                        // Count duplicates towards total removed count so they aren't copied as survived.
                        // This will also count towards aggregated evictions count which is ok.
                        entriesRemovedCount++;
                    }
                }
                else
                {
                    entriesSurvivedIndexes[entriesSurvivedCount] = i;
                    entriesSurvivedCount++;
                }
            }

            if (entriesSurvivedCount == 0)
            {
                Reset();

                ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);
                CacheManager.ReportEvictions<T>(entriesRemovedCount);
#if DEBUG
                Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.ElapsedTicks / 10} us");
#endif
                return entriesRemovedCount >= totalCount;
            }

            if (entriesRemovedCount == 0)
            {
                ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);
#if DEBUG
                Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.ElapsedTicks / 10} us");
#endif
                return entriesSurvivedCount >= totalCount;
            }

            var entriesSurvived = _inactive;
            for (uint j = 0; j < entriesSurvivedCount; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                entriesSurvived[j] = entries[entryIndex];
            }

            // Set inactive backing array where we stored survived entries as active and update entries counter accordingly.
            // In-flight writes between active-inactive swap and counter update will be missed which is by design and
            // will be handled by the next full eviction if expired.
            _inactive = Interlocked.Exchange(ref _active, _inactive);
            Volatile.Write(ref _count, entriesSurvivedCount);

            ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);
            CacheManager.ReportEvictions<T>(entriesRemovedCount);

#if DEBUG
            Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.ElapsedTicks / 10} us");
#endif
            return (entriesSurvivedCount + entriesRemovedCount) >= totalCount;
        }
    }
}
