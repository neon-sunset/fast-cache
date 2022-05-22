using System.Buffers;
using System.Diagnostics;
using FastCache.Services;

namespace FastCache;

internal sealed class EvictionQuickList<T>
{
    private static readonly SemaphoreSlim s_evictionLock = new(1, 1);

    private (int, long)[] _active;
    private (int, long)[] _inactive;
    private ulong _count;

    public uint Count => (uint)Interlocked.Read(ref _count);

    public EvictionQuickList()
    {
        _active = ArrayPool<(int, long)>.Shared.Rent(checked((int)Constants.QuickListMinLength));
        _inactive = ArrayPool<(int, long)>.Shared.Rent(checked((int)Constants.QuickListMinLength));
        _count = 0;
    }

    /// <summary>
    /// Atomic, will never perform out of bounds access on AtomicSwapActive
    /// </summary>
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

    /// <summary>
    /// Atomic-compatible with AtomicSwapActive, writes are not atomically visible however which is by design.
    /// </summary>
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
    internal bool Evict(long now, bool resize = false)
    {
#if FASTCACHE_DEBUG
        var sw = Stopwatch.StartNew();
#endif
        var store = Cached<T>.s_store;

        if (!s_evictionLock.Wait(0))
        {
            return false;
        }

        var totalCount = store.Count;
        if (totalCount is 0)
        {
            if (_inactive.Length != Constants.QuickListMinLength)
            {
                ResizeInactive((int)Constants.QuickListMinLength);
                AtomicSwapActive(0);
            }

            s_evictionLock.Release();
            return true;
        }
        else if (_count is 0)
        {
            if (resize)
            {
                ResizeInactive(CalculateResize(totalCount));
                AtomicSwapActive(0);
            }

            s_evictionLock.Release();
            return false;
        }

        var needsResizing = false;
        var resizedLength = 0;
        if (resize)
        {
            resizedLength = (int)((double)Constants.QuickListAdjustableLengthRatio / 100 * totalCount);
            needsResizing = resizedLength > Constants.QuickListMinLength;
        }
        else if (_active.Length != _inactive.Length)
        {
            resizedLength = _active.Length;
            needsResizing = true;
        }

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
            s_evictionLock.Release();

#if FASTCACHE_DEBUG
            Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.Elapsed.Ticks / 10} us");
#endif
            return entriesRemovedCount >= totalCount;
        }

        if (entriesRemovedCount == 0)
        {
            ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);
            s_evictionLock.Release();

#if FASTCACHE_DEBUG
            Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.Elapsed.Ticks / 10} us");
#endif
            return entriesSurvivedCount >= totalCount;
        }

        var postEvictionCount = entriesSurvivedCount;
        if (needsResizing)
        {
            resizedLength = ResizeInactive(resizedLength);

            postEvictionCount = Math.Min(entriesSurvivedCount, (uint)resizedLength);
        }

        // Use 'inactive' replacement array to store survived items.
        // In addition, if survived items exceed resized _inactive array length,
        // just drop the rest that didn't fit - they will be handled by full eviction.
        var entriesSurvived = _inactive;
        for (uint j = 0; j < postEvictionCount; j++)
        {
            var entryIndex = entriesSurvivedIndexes[j];
            entriesSurvived[j] = entries[entryIndex];
        }

        ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);

        // Set inactive backing array where we stored survived entries as active and update entries counter accordingly.
        // In-flight writes between active-inactive swap and counter update will be missed which is by design and
        // will be handled by the next full eviction (evicted or pushed to quick list if capacity allows it).
        AtomicSwapActive(postEvictionCount);

        CacheManager.ReportEvictions<T>(entriesRemovedCount);
        s_evictionLock.Release();

#if FASTCACHE_DEBUG
        Console.WriteLine($"FastCache: Evicted {entriesRemovedCount} {typeof(T).Name} from quick list. Took {sw.Elapsed.Ticks / 10} us");
#endif
        return (entriesSurvivedCount + entriesRemovedCount) >= totalCount;
    }

    private static int CalculateResize(int totalCount)
    {
        return (int)((double)Constants.QuickListAdjustableLengthRatio / 100 * totalCount);
    }

    private int ResizeInactive(int requestedLength)
    {
        if (requestedLength == _inactive.Length)
        {
            return _inactive.Length;
        }

        ArrayPool<(int, long)>.Shared.Return(_inactive);
        _inactive = ArrayPool<(int, long)>.Shared.Rent(requestedLength);

#if FASTCACHE_DEBUG
        Console.WriteLine($"FastCache: _inactive has been resized. New length: {_inactive.Length}");
#endif
        return _inactive.Length;
    }

    private void AtomicSwapActive(uint postEvictionCount)
    {
        _inactive = Interlocked.Exchange(ref _active, _inactive);
        Volatile.Write(ref _count, postEvictionCount);
    }
}
