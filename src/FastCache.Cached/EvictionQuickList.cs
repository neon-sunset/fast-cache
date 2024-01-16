using System.Buffers;
using System.Diagnostics;
using FastCache.Helpers;
using FastCache.Services;

namespace FastCache;

internal sealed class EvictionQuickList<K, V> : IDisposable where K : notnull
{
    private readonly SemaphoreSlim _evictionLock = new(1, 1);

    private (K Key, long Timestamp)[] _active;
    private (K Key, long Timestamp)[] _inactive;
    private long _count;

    public uint AtomicCount => (uint)Interlocked.Read(ref _count);

    public uint FreeSpace => (uint)_active.Length - AtomicCount;

    public bool InProgress => _evictionLock.CurrentCount == 0;

    public EvictionQuickList()
    {
        _active = ArrayPool<(K, long)>.Shared.Rent(Constants.QuickListMinLength);
        _inactive = ArrayPool<(K, long)>.Shared.Rent(Constants.QuickListMinLength);
        _count = 0;
    }

    /// <summary>
    /// Atomic, will never perform out of bounds access on AtomicSwapActive
    /// </summary>
    public void Add(K key, long expiresAt)
    {
        var entries = _active;
        var count = (uint)(ulong)_count;
        if (count < entries.Length)
        {
            entries[count] = (key, expiresAt);
            if (!TypeInfo.IsManaged<K>())
            {
                Interlocked.MemoryBarrier();
            }
            _count = count + 1;
        }
    }

    public void Reset() => Reset(lockRequired: true);

    /// <summary>
    /// Atomic-compatible with AtomicSwapActive, writes are not atomically visible however which is by design.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void OverwritingAdd(K key, long expiresAt)
    {
        var entries = _active;
        var count = (uint)(ulong)_count;
        if (count < entries.Length)
        {
            entries[count] = (key, expiresAt);
            _count = count + 1;
        }
    }

    // Performs cache eviction by iterating through quick list and removing expired entries from cache store.
    // Returns 'true' if resident cache size is contained within quick list, 'false' if full eviction is required
    internal bool Evict(bool resize = false)
    {
#if FASTCACHE_DEBUG
        var sw = Stopwatch.StartNew();
#endif

        var now = TimeUtils.Now;
        var store = CacheStaticHolder<K, V>.Store;

        if (!_evictionLock.Wait(0))
        {
            return false;
        }

        var totalCount = store.Count;
        if (totalCount is 0)
        {
            if (resize || _inactive.Length != Constants.QuickListMinLength)
            {
                ResizeInactive(Constants.QuickListMinLength);
                AtomicSwapActive(0);
            }
            else
            {
                Reset(lockRequired: false);
            }

            _evictionLock.Release();
            return true;
        }
        else if (AtomicCount is 0)
        {
            if (resize)
            {
                ResizeInactive(CalculateResize(totalCount));
                AtomicSwapActive(0);
            }

            _evictionLock.Release();
            return false;
        }

        var needsResizing = false;
        var resizedLength = 0;
        if (resize)
        {
            resizedLength = CalculateResize(totalCount);
            needsResizing = resizedLength > Constants.QuickListMinLength;
        }
        else if (_active.Length != _inactive.Length)
        {
            resizedLength = _active.Length;
            needsResizing = true;
        }

        var entries = _active;
        var entriesCount = AtomicCount;

        var entriesSurvivedIndexes = ArrayPool<uint>.Shared.Rent((int)entriesCount);

        uint entriesRemovedCount = 0;
        uint entriesSurvivedCount = 0;

        for (uint i = 0; i < entriesCount; i++)
        {
            var (key, expiresAt) = entries[i];

            if (now > expiresAt)
            {
                if (store.TryGetValue(key, out var inner))
                {
                    var itemTimestamp = inner._timestamp;
                    if (now > itemTimestamp)
                    {
                        store.TryRemove(key, out _);
                        entriesRemovedCount++;
                    }
                    else
                    {
                        entries[i] = (key, itemTimestamp);
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
            ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);

            if (needsResizing)
            {
                ResizeInactive(resizedLength);
                AtomicSwapActive(0);
            }
            else
            {
                Reset(lockRequired: false);
            }

            CacheManager.ReportEvictions(entriesRemovedCount);
            _evictionLock.Release();

#if FASTCACHE_DEBUG
            PrintEvicted(sw.Elapsed, entriesRemovedCount);
#endif
            return entriesRemovedCount >= totalCount;
        }

        if (entriesRemovedCount == 0)
        {
            ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);

            if (needsResizing)
            {
                ResizeInactive(resizedLength, copy: true);
                AtomicSwapActive(entriesSurvivedCount);
            }

            _evictionLock.Release();

#if FASTCACHE_DEBUG
            PrintEvicted(sw.Elapsed, entriesRemovedCount);
#endif
            return entriesSurvivedCount >= totalCount;
        }

        if (needsResizing)
        {
            ResizeInactive(resizedLength);
        }

        // Use 'inactive' replacement array to store survived items.
        // In addition, if survived items exceed resized _inactive array length,
        // just drop the rest that didn't fit - they will be handled by full eviction.
        var entriesSurvived = _inactive;
        var copyLength = Math.Min(entriesSurvivedCount, (uint)entriesSurvived.Length);
        for (uint j = 0; j < copyLength; j++)
        {
            var entryIndex = entriesSurvivedIndexes[j];
            entriesSurvived[j] = entries[entryIndex];
        }

        ArrayPool<uint>.Shared.Return(entriesSurvivedIndexes);

        // Set inactive backing array where we stored survived entries as active and update entries counter accordingly.
        // In-flight writes between active-inactive swap and counter update will be missed which is by design and
        // will be handled by the next full eviction (evicted or pushed to quick list if capacity allows it).
        AtomicSwapActive(copyLength);

        CacheManager.ReportEvictions(entriesRemovedCount);
        _evictionLock.Release();

#if FASTCACHE_DEBUG
        PrintEvicted(sw.Elapsed, entriesRemovedCount);
#endif
        return (entriesSurvivedCount + entriesRemovedCount) >= totalCount;
    }

    internal uint Trim(uint count)
    {
        if (!_evictionLock.Wait(0))
        {
            return 0;
        }

        var active = _active;
        uint currentCount = AtomicCount;
        uint toTrim = Math.Min(currentCount, count);

        var trimEntries = active.AsSpan(
            (int)(currentCount - toTrim), (int)toTrim);

        if (trimEntries.IsEmpty)
        {
            return 0;
        }

        var removed = 0;
        var store = CacheStaticHolder<K, V>.Store;
        foreach (var (key, _) in trimEntries)
        {
            if (store.TryRemove(key, out _))
            {
                removed++;
            }
        }

        Interlocked.Exchange(ref _count, currentCount - toTrim);
        _evictionLock.Release();
        return (uint)removed;
    }

    internal void PullFromCacheStore()
    {
        uint i = 0;
        uint limit = FreeSpace;

        foreach (var (key, inner) in CacheStaticHolder<K, V>.Store)
        {
            if (i >= limit)
            {
                break;
            }

            OverwritingAdd(key, inner._timestamp);
            i++;
        }
    }

    internal bool TryLock()
    {
        return _evictionLock.Wait(0);
    }

    internal void Release()
    {
        _evictionLock.Release();
    }

    private static int CalculateResize(long totalCount)
    {
        return Math.Max(
            (int)(Constants.QuickListAdjustableLengthPercentage / 100D * totalCount),
            Constants.QuickListMinLength);
    }

    // TODO: Refactor into 'ResizeAndSwap'
    private int ResizeInactive(int requestedLength, bool copy = false)
    {
        // Opt. opportunity: round up requestedLength to the next PowOf2
        // so that we don't return and then rent the array when there is no need to
        if (requestedLength == _inactive.Length)
        {
            return _inactive.Length;
        }

        ArrayPool<(K, long)>.Shared.Return(_inactive, TypeInfo.IsManaged<K>());
        _inactive = ArrayPool<(K, long)>.Shared.Rent(requestedLength);

        if (copy)
        {
            var length = Math.Min(AtomicCount, _inactive.Length);
            Array.Copy(_active, _inactive, length);
        }

#if FASTCACHE_DEBUG
        Console.WriteLine($"FastCache: _inactive for {_inactive.GetType()} has been resized. New length: {_inactive.Length}");
#endif
        return _inactive.Length;
    }

    private void AtomicSwapActive(uint postEvictionCount)
    {
        _inactive = Interlocked.Exchange(ref _active, _inactive);
        Interlocked.Exchange(ref _count, postEvictionCount);
    }

    private void Reset(bool lockRequired)
    {
        if (lockRequired)
        {
            _evictionLock.Wait();
        }

        if (TypeInfo.IsManaged<K>())
        {
            _active.AsSpan().Clear();
        }

        Interlocked.Exchange(ref _count, 0);

        if (lockRequired)
        {
            _evictionLock.Release();
        }
    }

    public void Dispose()
    {
        ArrayPool<(K, long)>.Shared.Return(_active);
        ArrayPool<(K, long)>.Shared.Return(_inactive);
    }

#if FASTCACHE_DEBUG
    private void PrintEvicted(TimeSpan elapsed, uint evictedCount)
    {
        Console.WriteLine(
            $"FastCache: Evicted {evictedCount} {typeof(K).Name}:{typeof(V).Name} from quick list. Size after: {AtomicCount}, took {elapsed.Ticks / 10} us");
    }
#endif
}
