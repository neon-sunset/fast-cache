using System.Buffers;
using System.Diagnostics;
using FastCache.Helpers;

namespace FastCache.Services;

public static class CacheManager
{
    private static readonly SemaphoreSlim FullGCLock = new(1, 1);

    private static long s_AggregatedEvictionsCount;

    /// <summary>
    /// Submit full eviction for specified Cached<K, V> value
    /// </summary>
    public static void QueueFullEviction<K, V>() where K : notnull => QueueFullEviction<K, V>(triggeredByTimer: true);

    public static void QueueFullClear<K, V>() where K : notnull
    {
        ThreadPool.QueueUserWorkItem(async static _ =>
        {
            var evictionJob = CacheStaticHolder<K, V>.EvictionJob;
            await evictionJob.FullEvictionLock.WaitAsync();

#if FASTCACHE_DEBUG
            var countBefore = CacheStaticHolder<K, V>.Store.Count;
#endif

            CacheStaticHolder<K, V>.Store.Clear();
            CacheStaticHolder<K, V>.QuickList.Reset();

            evictionJob.FullEvictionLock.Release();

#if FASTCACHE_DEBUG
            Console.WriteLine(
                $"FastCache: Cache has been fully cleared for {typeof(K).Name}:{typeof(V).Name}. Was {countBefore}, now {CacheStaticHolder<K, V>.QuickList.AtomicCount}/{CacheStaticHolder<K, V>.Store.Count}");
#endif
        });
    }

    /// <summary>
    /// Suspends automatic eviction. Does not affect already in-flight operations.
    /// </summary>
    public static void SuspendEviction<K, V>() where K : notnull => CacheStaticHolder<K, V>.EvictionJob.Stop();

    /// <summary>
    /// Resumes eviction, next iteration will occur after a standard adaptive interval from now.
    /// Is a no-op if automatic eviction is disabled.
    /// </summary>
    public static void ResumeEviction<K, V>() where K : notnull => CacheStaticHolder<K, V>.EvictionJob.Resume();

    internal static void ReportEvictions<T>(uint count)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() && Constants.ConsiderFullGC)
        {
            Interlocked.Add(ref s_AggregatedEvictionsCount, count);
        }
    }

    internal static void QueueFullEviction<K, V>(bool triggeredByTimer) where K : notnull
    {
        if (!CacheStaticHolder<K, V>.EvictionJob.IsActive)
        {
            return;
        }

        if (triggeredByTimer)
        {
            ThreadPool.QueueUserWorkItem(static _ =>
            {
                try
                {
                    ImmediateFullEviction<K, V>();
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
                    await StaggeredFullEviction<K, V>();
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

    private static void ImmediateFullEviction<K, V>() where K : notnull
    {
        var evictionJob = CacheStaticHolder<K, V>.EvictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        evictionJob.RescheduleConsideringExpiration();

        if (CacheStaticHolder<K, V>.QuickList.Evict(resize: true))
        {
            evictionJob.FullEvictionLock.Release();
            return;
        }

#if FASTCACHE_DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromCacheStore = EvictFromCacheStore<K, V>();

        if (Constants.ConsiderFullGC && evictedFromCacheStore > 0)
        {
            ReportEvictions<V>(evictedFromCacheStore);
        }

#if FASTCACHE_DEBUG
        PrintEvicted<K, V>(evictedFromCacheStore, stopwatch.Elapsed);
#endif

        ThreadPool.QueueUserWorkItem(async static _ => await ConsiderFullGC<V>());

        evictionJob.FullEvictionLock.Release();
    }

    private static async ValueTask StaggeredFullEviction<K, V>() where K : notnull
    {
        var evictionJob = CacheStaticHolder<K, V>.EvictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        if (CacheStaticHolder<K, V>.QuickList.Evict())
        {
            // When a lot of items are being added to cache, it triggers GC
            // which may decrease adding performance by constantly locking quick list.
            // Avoid this by holding full eviction lock by additional 1000 ms (for 5s quicklist inerval).
            await Task.Delay(Constants.EvictionCooldownDelayOnGC);
            evictionJob.FullEvictionLock.Release();
            return;
        }

        evictionJob.EvictionGCNotificationsCount++;
        if (evictionJob.EvictionGCNotificationsCount < 2)
        {
            await Task.Delay(Constants.EvictionCooldownDelayOnGC);
            evictionJob.FullEvictionLock.Release();
            return;
        }

        await Task.Delay(Constants.CacheStoreEvictionDelay);

#if FASTCACHE_DEBUG
        var stopwatch = Stopwatch.StartNew();
#endif
        var evictedFromCacheStore = EvictFromCacheStore<K, V>();

        if (Constants.ConsiderFullGC && evictedFromCacheStore > 0)
        {
            ReportEvictions<V>(evictedFromCacheStore);
        }

#if FASTCACHE_DEBUG
        PrintEvicted<K, V>(evictedFromCacheStore, stopwatch.Elapsed);
#endif

        await Task.Delay(Constants.EvictionCooldownDelayOnGC);

        evictionJob.EvictionGCNotificationsCount = 0;
        evictionJob.FullEvictionLock.Release();
    }

    private static uint EvictFromCacheStore<K, V>() where K : notnull
    {
        var evictedCount = CacheStaticHolder<K, V>.Store.Count > Constants.ParallelEvictionThreshold
            ? EvictFromCacheStoreParallel<K, V>()
            : EvictFromCacheStoreSingleThreaded<K, V>();

        CacheStaticHolder<K, V>.QuickList.PullFromCacheStore();

        return evictedCount;
    }

    private static uint EvictFromCacheStoreSingleThreaded<K, V>() where K : notnull
    {
        var now = TimeUtils.Now;
        var store = CacheStaticHolder<K, V>.Store;
        uint totalRemoved = 0;

        foreach (var (key, value) in store)
        {
            if (now > value._timestamp)
            {
                store.TryRemove(key, out _);
                totalRemoved++;
            }
        }

        return totalRemoved;
    }

    private static uint EvictFromCacheStoreParallel<K, V>() where K : notnull
    {
        var now = TimeUtils.Now;
        uint totalRemoved = 0;

        void CheckAndRemove(K key, long timestamp)
        {
            ref var count = ref totalRemoved;

            if (now > timestamp)
            {
                CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
                count++;
            }
        }

        CacheStaticHolder<K, V>.Store
            .AsParallel()
            .AsUnordered()
            .ForAll(item => CheckAndRemove(item.Key, item.Value._timestamp));

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
    private static void PrintEvicted<K, V>(uint count, TimeSpan elapsed) where K : notnull
    {
        var size = CacheStaticHolder<K, V>.Store.Count;
        Console.WriteLine(
            $"FastCache: Evicted {count} of {typeof(K).Name}:{typeof(V).Name} from cache store. Size after: {size}, took {elapsed.TotalMilliseconds} ms.");
    }
#endif
}
