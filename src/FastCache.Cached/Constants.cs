using FastCache.Helpers;

namespace FastCache;

internal static class Constants
{
    /// <summary>
    /// Min length: 32 * 128 default multiplier = 4096 elements. If changed, the value must be a power of 2.
    /// </summary>
    private const int DefaultQuickListMinLengthFactor = 8;
    private const uint DefaultQuickListAutoLengthPercent = 5;
    private const uint DefaultIntervalMultiplyFactor = 20;
    private const uint DefaultParallelEvictionThreshold = 1_572_864;
    private const uint DefaultParallelSaveMinBatchSize = 1024;
    private const uint DefaultAggregatedGCThreshold = 1_572_864;

    private static readonly TimeSpan DefaultQuickListInterval = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan MaxQuickListInterval = TimeSpan.FromSeconds(60);

#if !NET6_0_OR_GREATER
    private static readonly Random Random = new();
#endif

    public static readonly int QuickListMinLength = (int
        .TryParse(GetVar("FASTCACHE_QUICKLIST_MIN_LENGTH_FACTOR"), out var parsed) ? parsed : DefaultQuickListMinLengthFactor) * 128;

    public static readonly uint QuickListAdjustableLengthPercentage = uint
        .TryParse(GetVar("FASTCACHE_QUICKLIST_AUTO_LENGTH_PERCENT"), out var parsed) && parsed <= 25
            ? parsed : DefaultQuickListAutoLengthPercent;

    public static readonly TimeSpan QuickListEvictionInterval = TimeSpan
        .TryParse(GetVar("FASTCACHE_QUICKLIST_EVICTION_INTERVAL"), out var parsed)
            ? parsed < MaxQuickListInterval
                ? parsed
                : MaxQuickListInterval
            : DefaultQuickListInterval;

    public static readonly uint EvictionIntervalMultiplyFactor = uint
        .TryParse(GetVar("FASTCACHE_INTERVAL_MUL_FACTOR"), out var parsed) ? parsed : DefaultIntervalMultiplyFactor;

    public static readonly long AggregatedGCThreshold = long
        .TryParse(GetVar("FASTCACHE_GC_THRESHOLD"), out var parsed) ? parsed : DefaultAggregatedGCThreshold;

    public static readonly uint ParallelEvictionThreshold = uint
        .TryParse(GetVar("FASTCACHE_PARALLEL_EVICTION_THRESHOLD"), out var parsed) ? parsed : DefaultParallelEvictionThreshold;

    public static uint ParallelSaveMinBatchSize = uint
        .TryParse(GetVar("FASTCACHE_PARALLEL_SAVE_MIN_BATCH_SIZE"), out var parsed) ? parsed : DefaultParallelSaveMinBatchSize;

    public static double FullCapacityTrimPercentage = double
        .TryParse(GetVar("FASTCACHE_FULL_CAPACITY_TRIM_PERCENT"), out var parsed) ? parsed : QuickListAdjustableLengthPercentage;

    public static uint InlineTrimCountThreshold = uint
        .TryParse(GetVar("FASTCACHE_INLINE_TRIM_COUNT_THRESHOLD"), out var parsed) ? parsed : 256;

    public static readonly bool ConsiderFullGC = bool.TryParse(GetVar("FASTCACHE_CONSIDER_GC"), out var parsed) && parsed;

    public static readonly bool DisableEvictionJob = bool.TryParse(GetVar("FASTCACHE_DISABLE_AUTO_EVICTION"), out var parsed) && parsed;

    // Full eviction interval uses a multiple of quick list eviction interval.
    // Rationale: if cache size is larger than quick list, then running full eviction too often will cause
    // performance stalls and thrashing. For situations where items are added to cache faster than
    // eviction job can remove them, Gen2 GC callback is used to clear the cache before next scheduled run.
    // This allows us to avoid OOM situations while keeping full evictions sufficiently infrequent.
    // Hopefully, this is enough to prevent unnecessary performance overhead and interruptions to application behavior.
    public static TimeSpan FullEvictionInterval
    {
        get
        {
            // Calculate full eviction interval with jitter.
            // This is necessary to avoid application stalling induced by all caches getting collected at the same time.
            var quickListTicks = (int)QuickListEvictionInterval.TotalMilliseconds;
            var delay = quickListTicks * EvictionIntervalMultiplyFactor;
            var jitter = GetRandomInt(-quickListTicks, (quickListTicks * 2) + 1);
            return TimeSpan.FromMilliseconds(delay + jitter);
        }
    }

    public static TimeSpan CacheStoreEvictionDelay
    {
        get
        {
            var delay = (int)QuickListEvictionInterval.TotalMilliseconds;
            var jitter = GetRandomInt(0, delay * 2);
            return TimeSpan.FromMilliseconds(delay + jitter);
        }
    }

    public static readonly TimeSpan EvictionCooldownDelayOnGC = QuickListEvictionInterval.DivideBy(5);

    public static readonly TimeSpan DelayToFullGC = QuickListEvictionInterval.MultiplyBy(4);
    public static readonly TimeSpan CooldownDelayAfterFullGC = QuickListEvictionInterval.MultiplyBy(4);

    private static string? GetVar(string key) => Environment.GetEnvironmentVariable(key);

    private static int GetRandomInt(int minValue, int maxValue)
    {
#if NET6_0_OR_GREATER
        return Random.Shared.Next(minValue, maxValue);
#else
        lock (Random)
        {
            return Random.Next(minValue, maxValue);
        }
#endif
    }
}
