namespace FastCache;

internal static class Constants
{
    private const int DefaultQuickListMinLengthFactor = 128;
    private const uint DefaultQuickListAutoLengthPercent = 10;
    private const uint DefaultIntervalMultiplyFactor = 15;
    private const uint DefaultParallelEvictionThreshold = 1_048_576;
    private const uint DefaultAggregatedGCThreshold = 1_572_864;

    private static readonly TimeSpan DefaultQuickListInterval = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan MaxQuickListInterval = TimeSpan.FromSeconds(60);

    // OldestEntries list and eviction batch size length limits.
    // Higher limit works well with short-lived first-gen-contained cache items
    // but performs poorly if many items of the same type have inconsistent lifetimes.
    public static readonly int QuickListMinLength = (int
        .TryParse(GetVar("FASTCACHE_QUICKLIST_MIN_LENGTH_FACTOR"), out var parsed) ? parsed : DefaultQuickListMinLengthFactor) * 128;

    public static readonly uint QuickListAdjustableLengthRatio = uint
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

    public static readonly ulong AggregatedGCThreshold = ulong
        .TryParse(GetVar("FASTCACHE_GC_THRESHOLD"), out var parsed) ? parsed : DefaultAggregatedGCThreshold;

    public static readonly uint ParallelEvictionThreshold = uint
        .TryParse(GetVar("FASTCACHE_PARALLEL_EVICTION_THRESHOLD"), out var parsed) ? parsed : DefaultParallelEvictionThreshold;

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
            var quickListTicks = QuickListEvictionInterval.Ticks;
            var delay = quickListTicks * EvictionIntervalMultiplyFactor;
            var jitter = Random.Shared.NextInt64(-quickListTicks, (quickListTicks * 2) + 1);
            return TimeSpan.FromTicks(delay + jitter);
        }
    }

    public static TimeSpan CacheStoreEvictionDelay
    {
        get
        {
            var delay = QuickListEvictionInterval.Ticks;
            var jitter = Random.Shared.NextInt64(0, delay * 2);
            return TimeSpan.FromTicks(delay + jitter);
        }
    }

    public static readonly TimeSpan EvictionCooldownDelayOnGC = QuickListEvictionInterval / 5;

    public static readonly TimeSpan DelayToFullGC = QuickListEvictionInterval * 4;
    public static readonly TimeSpan CooldownDelayAfterFullGC = QuickListEvictionInterval * 4;

    private static string? GetVar(string key) => Environment.GetEnvironmentVariable(key);
}
