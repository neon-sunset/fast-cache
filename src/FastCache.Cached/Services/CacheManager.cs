using System.Buffers;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);
    private static readonly Mutex GcMutex = new();

    public static bool QueueFullEviction<T>(bool triggeredByTimer) where T : notnull
    {
        return ThreadPool.QueueUserWorkItem(
            async static triggeredByTimer => await PerformFullEviction<T>(triggeredByTimer),
            triggeredByTimer,
            preferLocal: false);
    }

    internal static async ValueTask PerformFullEviction<T>(bool triggeredByTimer) where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;
        if (!evictionJob.FullEvictionLock.Wait(0))
        {
            return;
        }

        var now = DateTime.UtcNow.Ticks;

        // When a lot of items are being added to cache, it triggers GC
        // which may decrease adding performance by constantly locking quick list.
        // Avoid this by delaying quick list eviction by additional 100ms.
        await Task.Delay(Constants.QuickListEvictionDelay);
        if (EvictFromQuickList<T>(now))
        {
            evictionJob.FullEvictionLock.Release();
            return;
        }

        await Task.Delay(Constants.CacheStoreEvictionDelay);
        var evictedFromMainCache = EvictFromMainCache<T>(now);
        if (evictedFromMainCache > 0)
        {
            Interlocked.Add(ref evictionJob.ReportedEvictionsCount, evictedFromMainCache);
        }
        else if (triggeredByTimer)
        {
            evictionJob.EvictionBackoffCount++;

            if (evictionJob.EvictionBackoffCount < evictionJob.EvictionBackoffLimit)
            {
                var interval = Constants.FullEvictionInterval;
                for (int i = 0; i < evictionJob.EvictionBackoffCount; i++)
                {
                    interval += Constants.FullEvictionInterval;
                }

                evictionJob.FullEvictionTimer.Change(interval, interval);
            }
        }

        ThreadPool.QueueUserWorkItem(async static _ => await ConsiderFullGC<T>());
        evictionJob.FullEvictionLock.Release();
    }

    /// <summary>
    /// Returns true if resident cache size is contained within quicklist and full eviction is not required.
    /// </summary>
    internal static bool EvictFromQuickList<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var continueEviction = store.Count < Constants.CacheBufferSize;

        lock (Cached<T>.s_quickEvictList.Entries)
        {
            var quickListEntries = Cached<T>.s_quickEvictList.Entries;
            var entriesCount = Cached<T>.s_quickEvictList.Count;

            var entriesSurvivedIndexes = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize);

            var entriesRemovedCount = 0;
            var entriesSurvivedCount = 0;

            for (var i = 0; i < entriesCount; i++)
            {
                var (identifier, expiresAtTicks) = quickListEntries[i];

                if (now > expiresAtTicks)
                {
                    store.Remove(identifier, out _);
                    entriesRemovedCount++;
                }
                else
                {
                    entriesSurvivedIndexes[entriesSurvivedCount] = i;
                    entriesSurvivedCount++;
                }
            }

            if (entriesSurvivedCount == 0)
            {
                Cached<T>.s_quickEvictList.Reset();
                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
                Interlocked.Add(ref Cached<T>.s_evictionJob.ReportedEvictionsCount, entriesRemovedCount);

                return continueEviction;
            }

            if (entriesRemovedCount == 0)
            {
                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);

                return continueEviction;
            }

            var entriesSurvived = ArrayPool<(int, long)>.Shared.Rent(Constants.CacheBufferSize);
            for (var j = 0; j < entriesSurvivedCount; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                entriesSurvived[j] = quickListEntries[entryIndex];
            }

            Cached<T>.s_quickEvictList.Replace(entriesSurvived, entriesSurvivedCount);
            ArrayPool<(int, long)>.Shared.Return(quickListEntries);
            ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
            Interlocked.Add(ref Cached<T>.s_evictionJob.ReportedEvictionsCount, entriesRemovedCount);

            return continueEviction;
        }
    }

    private static int EvictFromMainCache<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var buffer = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize * 2);
        var bufferLength = buffer.Length;
        var totalRemoved = 0;

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (now > cacheItem.ExpiresAtTicks)
                {
                    buffer[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;

                    if (expiredEntriesCount >= bufferLength)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(buffer[i], out _);
                totalRemoved++;
            }

            if (expiredEntriesCount < bufferLength)
            {
                break;
            }
        }

        ArrayPool<int>.Shared.Return(buffer);

        return totalRemoved;
    }

    private static async ValueTask ConsiderFullGC<T>() where T : notnull
    {
        if (Cached<T>.s_evictionJob.ReportedEvictionsCount <= Constants.CacheBufferSize)
        {
            return;
        }

        if (!FullGCLock.Wait(0))
        {
            return;
        }

        await Task.Delay(Constants.GarbageCollectionDelay);
        GC.Collect(2, GCCollectionMode.Forced, blocking: false, compacting: true);

        FullGCLock.Release();
    }
}
