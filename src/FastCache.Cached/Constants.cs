namespace FastCache;

internal static class Constants
{
    // OldestEntries list and eviction batch size length limits.
    // Higher limit works well with short-lived first-gen-contained cache items
    // but performs poorly if many items of the same type have inconsistent lifetimes.
    public static readonly int CacheBufferSize = int
        .TryParse(GetVar("FASTCACHE_QUICKLIST_LENGTH"), out var parsed)
            ? parsed : 32768;

    // Frequency with which CacheItemsEvictionJob is run. Scheduling it often is only recommended when cache consists of
    // numerous frequently added short-lived items and not running it often enough will result in high memory usage.
    public static readonly TimeSpan QuickListEvictionInterval = TimeSpan
        .TryParse(GetVar("FASTCACHE_QUICKLIST_EVICTION_INTERVAL"), out var parsed)
            ? parsed : TimeSpan.FromSeconds(5);

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
            var totalTicks = quickListTicks * 10;
            var jitter = Random.Shared.NextInt64(-quickListTicks, quickListTicks + 1);
            return TimeSpan.FromTicks(totalTicks + jitter);
        }
    }

    private static string? GetVar(string key) => Environment.GetEnvironmentVariable(key);
}
