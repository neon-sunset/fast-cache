using System.Buffers;
using System.Diagnostics;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);

    private static ulong s_AggregatedEvictionsCount;

    public static bool QueueFullEviction<T>(bool triggeredByTimer) where T : notnull
    {
        return triggeredByTimer
            ? ThreadPool.QueueUserWorkItem(static _ => ImmediateFullEvictionByTimer<T>())
            : ThreadPool.QueueUserWorkItem(async static _ => await StaggeredFullEvictionByGC<T>());
    }

    private static void ImmediateFullEvictionByTimer<T>() where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        var now = DateTime.UtcNow.Ticks;

        if (EvictFromQuickList<T>(now))
        {
            evictionJob.FullEvictionLock.Release();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var evictedFromMainCache = EvictFromMainCache<T>(now);
        if (evictedFromMainCache > 0)
        {
            ReportEvicted<T>("cache store", evictedFromMainCache, stopwatch.ElapsedMilliseconds);
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)evictedFromMainCache);
        }
        else if (evictionJob.EvictionBackoffCount < Constants.EvictionBackoffLimit)
        {
            evictionJob.EvictionBackoffCount++;

            var interval = Constants.FullEvictionInterval;
            for (var i = 0; i < evictionJob.EvictionBackoffCount; i++)
            {
                interval += Constants.FullEvictionInterval;
            }

            Console.WriteLine($"FastCache: full eviction backoff for {typeof(T).Name}. New interval {interval}");
            evictionJob.FullEvictionTimer.Change(interval, interval);
        }

        ThreadPool.QueueUserWorkItem(async static _ => await ConsiderFullGC<T>());

        evictionJob.FullEvictionLock.Release();
    }

    private static async ValueTask StaggeredFullEvictionByGC<T>() where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!await evictionJob.FullEvictionLock.WaitAsync(millisecondsTimeout: 0))
        {
            return;
        }

        var now = DateTime.UtcNow.Ticks;

        if (EvictFromQuickList<T>(now))
        {
            // When a lot of items are being added to cache, it triggers GC
            // which may decrease adding performance by constantly locking quick list.
            // Avoid this by holding full eviction lock by additional 500 ms.
            await Task.Delay(Constants.EvictionCooldownDelayOnGC);
            evictionJob.FullEvictionLock.Release();
            return;
        }

        Interlocked.Add(ref evictionJob.EvictionGCNotificationsCount, 1);
        if (evictionJob.EvictionGCNotificationsCount < 3)
        {
            await Task.Delay(Constants.EvictionCooldownDelayOnGC);
            evictionJob.FullEvictionLock.Release();
            return;
        }

        await Task.Delay(Constants.CacheStoreEvictionDelay);

        var stopwatch = Stopwatch.StartNew();
        var evictedFromMainCache = EvictFromMainCache<T>(now);
        if (evictedFromMainCache > 0)
        {
            ReportEvicted<T>("cache store", evictedFromMainCache, stopwatch.ElapsedMilliseconds);
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)evictedFromMainCache);
        }

        await Task.Delay(Constants.EvictionCooldownDelayOnGC);

        Interlocked.Exchange(ref evictionJob.EvictionGCNotificationsCount, 0);
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
                Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)entriesRemovedCount);

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
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)entriesRemovedCount);

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
        if (s_AggregatedEvictionsCount <= Constants.AggregatedGCThreshold)
        {
            return;
        }

        if (!await FullGCLock.WaitAsync(millisecondsTimeout: 0))
        {
            return;
        }

        await Task.Delay(Constants.DelayToFullGC);
        GC.Collect(2, GCCollectionMode.Forced, blocking: false, compacting: true);

        Console.WriteLine($"FastCache: Full GC has been requested or ran, reported evictions count has been reset, was: {s_AggregatedEvictionsCount}. Source: {typeof(T).Name}");
        Interlocked.Exchange(ref s_AggregatedEvictionsCount, 0);

        await Task.Delay(Constants.CooldownDelayAfterFullGC);
        FullGCLock.Release();
    }

    private static void ReportEvicted<T>(string type, int count, long milliseconds) where T : notnull
    {
        Console.WriteLine($"FastCache: Evicted {count} of {typeof(T).Name} from {type} in {milliseconds}");
    }
}
