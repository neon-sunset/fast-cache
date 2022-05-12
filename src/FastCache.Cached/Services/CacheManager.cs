using System.Buffers;
using System.Diagnostics;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);

    private static ulong s_AggregatedEvictionsCount;

    /// <summary>
    /// Submit full eviction for specified Cached<T> value
    /// </summary>
    public static void QueueFullEviction<T>() where T : notnull => QueueFullEviction<T>(triggeredByTimer: false);

    public static void QueueFullClear<T>() where T : notnull
    {
        ThreadPool.QueueUserWorkItem(async static _ =>
        {
            var quickList = Cached<T>.s_quickEvictList;
            lock (quickList)
            {
                quickList.Reset();
            }

            var evictionJob = Cached<T>.s_evictionJob;
            await evictionJob.FullEvictionLock.WaitAsync();

            Cached<T>.s_store.Clear();

            evictionJob.FullEvictionLock.Release();
        });
    }

    internal static bool QueueFullEviction<T>(bool triggeredByTimer) where T : notnull
    {
        return triggeredByTimer
            ? ThreadPool.QueueUserWorkItem(static _ =>
                {
                    try
                    {
                        ImmediateFullEvictionByTimer<T>();
                    }
                    catch
                    {
#if DEBUG
                        throw;
#endif
                    }
                })
            : ThreadPool.QueueUserWorkItem(async static _ =>
                {
                    try
                    {
                        await StaggeredFullEvictionByGC<T>();
                    }
                    catch
                    {
#if DEBUG
                        throw;
#endif
                    }
                });
    }

    /// <summary>
    /// Returns true if resident cache size is contained within quick list and full eviction is not required.
    /// </summary>
    internal static bool EvictFromQuickList<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_store;
        var quickList = Cached<T>.s_quickEvictList;

        var totalCount = store.Count;
        if (totalCount is 0)
        {
            return true;
        }
        else if (quickList.Count is 0)
        {
            return false;
        }

        lock (quickList)
        {
            var quickListEntries = quickList.Entries;
            var entriesCount = quickList.Count;

            var entriesSurvivedIndexes = ArrayPool<int>.Shared.Rent(Constants.QuickListLength);

            var entriesRemovedCount = 0;
            var entriesSurvivedCount = 0;

            for (var i = 0; i < entriesCount; i++)
            {
                var (identifier, expiresAt) = quickListEntries[i];

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
                            quickListEntries[i] = (identifier, itemExpiresAt);
                            entriesSurvivedCount++;
                        }
                    }
                    else
                    {
                        // Duplicate entry present in quicklist has already been removed from cache store.
                        // Count duplicates towards total removed count so they aren't copied as survived.
                        // This will also count towards 's_AggregatedEvictionsCount' which is ok.
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
                quickList.Reset();

                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
                Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)entriesRemovedCount);

                return entriesRemovedCount >= totalCount;
            }

            if (entriesRemovedCount == 0)
            {
                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);

                return entriesSurvivedCount >= totalCount;
            }

            var entriesSurvived = quickList.Inactive;
            for (var j = 0; j < entriesSurvivedCount; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                entriesSurvived[j] = quickListEntries[entryIndex];
            }

            quickList.SwapOnEviction(entriesSurvivedCount);

            ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)entriesRemovedCount);

            return (entriesSurvivedCount + entriesRemovedCount) >= totalCount;
        }
    }

    private static void ImmediateFullEvictionByTimer<T>() where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        evictionJob.RescheduleTimers();

        var now = Environment.TickCount64;
        if (EvictFromQuickList<T>(now))
        {
            evictionJob.FullEvictionLock.Release();
            return;
        }

#if DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromMainCache = EvictFromMainCache<T>(now);

        if (Constants.ConsiderFullGC && evictedFromMainCache > 0)
        {
#if DEBUG
            ReportEvicted<T>("cache store", evictedFromMainCache, stopwatch.Elapsed);
#endif
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)evictedFromMainCache);
        }

        ThreadPool.QueueUserWorkItem(async static _ => await ConsiderFullGC<T>());

        evictionJob.FullEvictionLock.Release();
    }

    private static async ValueTask StaggeredFullEvictionByGC<T>() where T : notnull
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        if (EvictFromQuickList<T>(Environment.TickCount64))
        {
            // When a lot of items are being added to cache, it triggers GC
            // which may decrease adding performance by constantly locking quick list.
            // Avoid this by holding full eviction lock by additional 1000 ms (for 5s quicklist inerval).
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

#if DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromMainCache = EvictFromMainCache<T>(Environment.TickCount64);

        if (Constants.ConsiderFullGC && evictedFromMainCache > 0)
        {
#if DEBUG
            ReportEvicted<T>("cache store", evictedFromMainCache, stopwatch.Elapsed);
#endif
            Interlocked.Add(ref s_AggregatedEvictionsCount, (ulong)evictedFromMainCache);
        }

        await Task.Delay(Constants.EvictionCooldownDelayOnGC);

        evictionJob.EvictionGCNotificationsCount = 0;
        evictionJob.FullEvictionLock.Release();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int EvictFromMainCache<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_store;
        var totalRemoved = 0;

        foreach (var (identifier, (_, expiresAt)) in store)
        {
            if (now > expiresAt)
            {
                store.TryRemove(identifier, out _);
                totalRemoved++;
            }
        }

        return totalRemoved;
    }

    private static async ValueTask ConsiderFullGC<T>() where T : notnull
    {
        if (!Constants.ConsiderFullGC)
        {
            return;
        }

        if (Interlocked.Read(ref s_AggregatedEvictionsCount) <= Constants.AggregatedGCThreshold)
        {
            return;
        }

        if (!FullGCLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        await Task.Delay(Constants.DelayToFullGC);
        GC.Collect(2, GCCollectionMode.Default, blocking: false);

#if DEBUG
        Debug.Print($"FastCache: Full GC has been requested or ran, reported evictions count has been reset, was: {s_AggregatedEvictionsCount}. Source: {typeof(T).Name}");
#endif
        Interlocked.Exchange(ref s_AggregatedEvictionsCount, 0);

        await Task.Delay(Constants.CooldownDelayAfterFullGC);
        FullGCLock.Release();
    }

#if DEBUG
    private static void ReportEvicted<T>(string type, int count, TimeSpan elapsed) where T : notnull
    {
        Debug.Print($"FastCache: Evicted {count} of {typeof(T).Name} from {type}. Took {elapsed.TotalMilliseconds} ms.");
    }
#endif
}
