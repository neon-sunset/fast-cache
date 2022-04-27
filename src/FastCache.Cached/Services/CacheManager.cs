using System.Buffers;
using System.Diagnostics;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);

    private static ulong s_AggregatedEvictionsCount;

    public static bool QueueFullEviction<T>() where T : notnull => QueueFullEviction<T>(triggeredByTimer: true);

    internal static bool QueueFullEviction<T>(bool triggeredByTimer) where T : notnull
    {
        return triggeredByTimer
            ? ThreadPool.QueueUserWorkItem(static _ =>
                {
                    try
                    {
                        ImmediateFullEvictionByTimer<T>();
                    }
                    catch (Exception ex)
                    {
                        ReportException<T>(ex);
                    }
                })
            : ThreadPool.QueueUserWorkItem(async static _ =>
                {
                    try
                    {
                        await StaggeredFullEvictionByGC<T>();
                    }
                    catch (Exception ex)
                    {
                        ReportException<T>(ex);
                    }
                });
    }

    private static void ImmediateFullEvictionByTimer<T>() where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        var now = Environment.TickCount;

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

        var now = Environment.TickCount;

        if (EvictFromQuickList<T>(now))
        {
            // When a lot of items are being added to cache, it triggers GC
            // which may decrease adding performance by constantly locking quick list.
            // Avoid this by holding full eviction lock by additional 500 ms.
            await Task.Delay(Constants.EvictionCooldownDelayOnGC);
            evictionJob.FullEvictionLock.Release();
            return;
        }

        evictionJob.EvictionGCNotificationsCount++;
        if (evictionJob.EvictionGCNotificationsCount < 4)
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

        evictionJob.EvictionGCNotificationsCount = 0;
        evictionJob.FullEvictionLock.Release();
    }

    /// <summary>
    /// Returns true if resident cache size is contained within quick list and full eviction is not required.
    /// </summary>
    internal static bool EvictFromQuickList<T>(int now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var totalCount = store.Count;
        if (totalCount is 0)
        {
            return true;
        }

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

                return entriesRemovedCount == totalCount;
            }

            if (entriesRemovedCount == 0)
            {
                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);

                return entriesSurvivedCount == totalCount;
            }

            var entriesSurvived = ArrayPool<(int, int)>.Shared.Rent(Constants.CacheBufferSize);
            for (var j = 0; j < entriesSurvivedCount; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                entriesSurvived[j] = quickListEntries[entryIndex];
            }

            Cached<T>.s_quickEvictList.Replace(entriesSurvived, entriesSurvivedCount);
            ArrayPool<(int, int)>.Shared.Return(quickListEntries);
            ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)entriesRemovedCount);

            return (entriesSurvivedCount + entriesRemovedCount) == totalCount;
        }
    }

    private static int EvictFromMainCache<T>(int now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var buffer = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize * 2);
        var bufferLength = buffer.Length;
        var totalRemoved = 0;

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, (_, expiresAt)) in store)
            {
                if (now > expiresAt)
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
        if (!Constants.ConsiderFullGC)
        {
            return;
        }

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

    private static void ReportException<T>(Exception ex)
    {
        Console.WriteLine($"FastCache: Hit an exception during eviction of {typeof(T).Name}! Message {ex.Message}\n{ex.StackTrace}");
    }
}
