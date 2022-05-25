using System.Buffers;
using System.Diagnostics;
using FastCache.Helpers;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);

    private static long s_AggregatedEvictionsCount;

    /// <summary>
    /// Submit full eviction for specified Cached<T> value
    /// </summary>
    public static void QueueFullEviction<T>() => QueueFullEviction<T>(triggeredByTimer: true);

    public static void QueueFullClear<T>()
    {
        ThreadPool.QueueUserWorkItem(async static _ =>
        {
            var quickList = Cached<T>.s_quickList;
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

    /// <summary>
    /// Suspends automatic eviction. Does not affect already in-flight operations.
    /// </summary>
    public static void SuspendEviction<T>() => Cached<T>.s_evictionJob.Stop();

    /// <summary>
    /// Resumes eviction, next iteration will occur after a standard adaptive interval from now.
    /// Is a no-op if automatic eviction is disabled.
    /// </summary>
    public static void ResumeEviction<T>() => Cached<T>.s_evictionJob.Resume();

    internal static void ReportEvictions<T>(uint count)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() && Constants.ConsiderFullGC)
        {
            Interlocked.Add(ref s_AggregatedEvictionsCount, count);
        }
    }

    internal static void QueueFullEviction<T>(bool triggeredByTimer)
    {
        if (!Cached<T>.s_evictionJob.IsActive)
        {
            return;
        }

        if (triggeredByTimer)
        {
            ThreadPool.QueueUserWorkItem(static _ =>
            {
                try
                {
                    ImmediateFullEviction<T>();
                }
                catch
                {
#if DEBUG
                    throw;
#endif
                }
            });
        }
        else
        {
            ThreadPool.QueueUserWorkItem(async static _ =>
            {
                try
                {
                    await StaggeredFullEviction<T>();
                }
                catch
                {
#if DEBUG
                    throw;
#endif
                }
            });
        }
    }

    private static void ImmediateFullEviction<T>()
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        evictionJob.RescheduleConsideringExpiration();

        if (Cached<T>.s_quickList.Evict(resize: true))
        {
            evictionJob.FullEvictionLock.Release();
            return;
        }

#if FASTCACHE_DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromCacheStore = EvictFromCacheStore<T>();

        if (Constants.ConsiderFullGC && evictedFromCacheStore > 0)
        {
            ReportEvictions<T>(evictedFromCacheStore);
        }

#if FASTCACHE_DEBUG
        PrintEvicted<T>("cache store", evictedFromCacheStore, stopwatch.Elapsed);
#endif

        ThreadPool.QueueUserWorkItem(async static _ => await ConsiderFullGC<T>());

        evictionJob.FullEvictionLock.Release();
    }

    private static async ValueTask StaggeredFullEviction<T>()
    {
        var evictionJob = Cached<T>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        if (Cached<T>.s_quickList.Evict())
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

#if FASTCACHE_DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromCacheStore = EvictFromCacheStore<T>();

        if (Constants.ConsiderFullGC && evictedFromCacheStore > 0)
        {
            ReportEvictions<T>(evictedFromCacheStore);
        }

#if FASTCACHE_DEBUG
        PrintEvicted<T>("cache store", evictedFromCacheStore, stopwatch.Elapsed);
#endif

        await Task.Delay(Constants.EvictionCooldownDelayOnGC);

        evictionJob.EvictionGCNotificationsCount = 0;
        evictionJob.FullEvictionLock.Release();
    }

    private static uint EvictFromCacheStore<T>()
    {
        return Cached<T>.s_store.Count > Constants.ParallelEvictionThreshold
            ? EvictFromCacheStoreParallel<T>()
            : EvictFromCacheStoreSingleThreaded<T>();
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization on supported platforms
    private static uint EvictFromCacheStoreSingleThreaded<T>()
    {
        var now = TimeUtils.Now;
        var store = Cached<T>.s_store;
        var quickList = Cached<T>.s_quickList;
        uint totalRemoved = 0;

        foreach (var (identifier, (_, expiresAt)) in store)
        {
            if (now > expiresAt)
            {
                store.TryRemove(identifier, out _);
                totalRemoved++;
            }
            else
            {
                quickList.OverwritingNonAtomicAdd(identifier, expiresAt);
            }
        }

        return totalRemoved;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization on supported platforms
    // TODO: Add backoff logic if not enough items expired compared to expected. Recalculate avg expiration?
    private static uint EvictFromCacheStoreParallel<T>()
    {
        var now = TimeUtils.Now;
        var store = Cached<T>.s_store;
        uint totalRemoved = 0;

        void CheckAndRemove(int identifier, long expiresAt)
        {
            ref var count = ref totalRemoved;

            if (now > expiresAt)
            {
                store!.TryRemove(identifier, out _);
                count++;
            }
            else
            {
                Cached<T>.s_quickList.OverwritingNonAtomicAdd(identifier, expiresAt);
            }
        }

        Cached<T>.s_store
            .AsParallel()
            .AsUnordered()
            .ForAll(item => CheckAndRemove(item.Key, item.Value._expiresAt));

        return totalRemoved;
    }

    private static async ValueTask ConsiderFullGC<T>()
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
#if FASTCACHE_DEBUG
        var sw = Stopwatch.StartNew();
#endif

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, blocking: false);

#if FASTCACHE_DEBUG
        Console.WriteLine($"FastCache: Full GC has been requested or ran, reported evictions count has been reset, was: {s_AggregatedEvictionsCount}. Source: {typeof(T).Name}. Elapsed:{sw.ElapsedMilliseconds} ms");
#endif
        Interlocked.Exchange(ref s_AggregatedEvictionsCount, 0);

        await Task.Delay(Constants.CooldownDelayAfterFullGC);
        FullGCLock.Release();
    }

#if FASTCACHE_DEBUG
    private static void PrintEvicted<T>(string type, uint count, TimeSpan elapsed)
    {
        Console.WriteLine($"FastCache: Evicted {count} of {typeof(T).Name} from {type}. Took {elapsed.TotalMilliseconds} ms.");
    }
#endif
}
