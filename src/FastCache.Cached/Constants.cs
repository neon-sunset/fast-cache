namespace FastCache;

internal static class Constants
{
    private const int DefaultCacheBufferSize = 32768;
    private static readonly TimeSpan DefaultQuickListInterval = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan MaxQuickListInterval = TimeSpan.FromSeconds(30);

    // OldestEntries list and eviction batch size length limits.
    // Higher limit works well with short-lived first-gen-contained cache items
    // but performs poorly if many items of the same type have inconsistent lifetimes.
    public static readonly int CacheBufferSize = int
        .TryParse(GetVar("FASTCACHE_QUICKLIST_LENGTH"), out var parsed) ? parsed : DefaultCacheBufferSize;

    // Frequency with which CacheItemsEvictionJob is run. Scheduling it often is only recommended when cache consists of
    // numerous frequently added short-lived items and not running it often enough will result in high memory usage.
    public static readonly TimeSpan QuickListEvictionInterval = TimeSpan
        .TryParse(GetVar("FASTCACHE_QUICKLIST_EVICTION_INTERVAL"), out var parsed)
            ? parsed < MaxQuickListInterval
                ? parsed
                : MaxQuickListInterval
            : DefaultQuickListInterval;

    public static readonly bool DisableEviction = bool.TryParse(GetVar("FASTCACHE_DISABLE_EVICTION"), out var parsed) && parsed;

    public static readonly bool ConsiderFullGC = !bool.TryParse(GetVar("FASTCACHE_CONSIDER_GC"), out var parsed) || parsed;

    // Full eviction interval uses a multiple of quick list eviction interval.
    // Rationale: if cache size is larger than quick list, then running full eviction too often will cause
    // performance stalls and thrashing anyway. For situations where items are added to cache faster than
    // eviction job can remove them, Gen2 GC callback is used to clear the cache before next scheduled run.
    // This allows us to avoid OOM situations while keeping full evictions sufficiently infrequent.
    // Hopefully, this is sufficient to prevent unnecessary performance overhead and interruptions to application behavior.
    public static TimeSpan FullEvictionInterval
    {
        get
        {
            // Calculate full eviction interval with jitter.
            // This is necessary to avoid application stalling induced by all caches getting collected at the same time.
            var quickListTicks = QuickListEvictionInterval.Ticks;
            var delay = quickListTicks * 10;
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

    public const int EvictionBackoffLimit = 5;

    public static readonly TimeSpan EvictionCooldownDelayOnGC = QuickListEvictionInterval / 5;
    public static readonly ulong AggregatedGCThreshold = (ulong)CacheBufferSize * 8;
    public static readonly TimeSpan DelayToFullGC = QuickListEvictionInterval * 4;
    public static readonly TimeSpan CooldownDelayAfterFullGC = QuickListEvictionInterval * 4;

    private static string? GetVar(string key) => Environment.GetEnvironmentVariable(key);
}
