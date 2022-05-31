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
            var evictionJob = CacheStaticHolder<K, V>.s_evictionJob;
            await evictionJob.FullEvictionLock.WaitAsync();

            CacheStaticHolder<K, V>.s_store.Clear();
            CacheStaticHolder<K, V>.s_quickList.Evict();

            evictionJob.FullEvictionLock.Release();
        });
    }

    /// <summary>
    /// Suspends automatic eviction. Does not affect already in-flight operations.
    /// </summary>
    public static void SuspendEviction<K, V>() where K : notnull => CacheStaticHolder<K, V>.s_evictionJob.Stop();

    /// <summary>
    /// Resumes eviction, next iteration will occur after a standard adaptive interval from now.
    /// Is a no-op if automatic eviction is disabled.
    /// </summary>
    public static void ResumeEviction<K, V>() where K : notnull => CacheStaticHolder<K, V>.s_evictionJob.Resume();

    internal static void ReportEvictions<T>(uint count)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() && Constants.ConsiderFullGC)
        {
            Interlocked.Add(ref s_AggregatedEvictionsCount, count);
        }
    }

    internal static void QueueFullEviction<K, V>(bool triggeredByTimer) where K : notnull
    {
        if (!CacheStaticHolder<K, V>.s_evictionJob.IsActive)
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
        var evictionJob = CacheStaticHolder<K, V>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        evictionJob.RescheduleConsideringExpiration();

        if (CacheStaticHolder<K, V>.s_quickList.Evict(resize: true))
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
        var evictionJob = CacheStaticHolder<K, V>.s_evictionJob;

        if (!evictionJob.FullEvictionLock.Wait(millisecondsTimeout: 0))
        {
            return;
        }

        if (CacheStaticHolder<K, V>.s_quickList.Evict())
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
        return CacheStaticHolder<K, V>.s_store.Count > Constants.ParallelEvictionThreshold
            ? EvictFromCacheStoreParallel<K, V>()
            : EvictFromCacheStoreSingleThreaded<K, V>();
    }

    private static uint EvictFromCacheStoreSingleThreaded<K, V>() where K : notnull
    {
        var now = TimeUtils.Now;
        var store = CacheStaticHolder<K, V>.s_store;
        var quickList = CacheStaticHolder<K, V>.s_quickList;
        uint totalRemoved = 0;

        foreach (var (identifier, value) in store)
        {
            var expiresAt = value._expiresAt;
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

    private static uint EvictFromCacheStoreParallel<K, V>() where K : notnull
    {
        var now = TimeUtils.Now;
        uint totalRemoved = 0;

        void CheckAndRemove(K key, long expiresAt)
        {
            ref var count = ref totalRemoved;

            if (now > expiresAt)
            {
                CacheStaticHolder<K, V>.s_store.TryRemove(key, out _);
                count++;
            }
            else
            {
                CacheStaticHolder<K, V>.s_quickList.OverwritingNonAtomicAdd(key, expiresAt);
            }
        }

        CacheStaticHolder<K, V>.s_store
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
    private static void PrintEvicted<K, V>(uint count, TimeSpan elapsed) where K : notnull
    {
        var size = CacheStaticHolder<K, V>.s_store.Count;
        Console.WriteLine(
            $"FastCache: Evicted {count} of {typeof(K).Name}:{typeof(V).Name} from cache store. Size after: {size}, took {elapsed.TotalMilliseconds} ms.");
    }
#endif
}
