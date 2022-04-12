namespace FastCache;

internal static class Constants
{
    // OldestEntries list and eviction batch size length limits.
    // Higher limit works well with short-lived first-gen-contained cache items
    // but performs poorly if many items of the same type have inconsistent lifetimes.
    public static readonly int CacheBufferSize = int.TryParse(GetVar("FASTCACHE_BUFFER_LENGTH"), out var parsed) ? parsed : 32768;

    // Frequency with which CacheItemsEvictionJob is run. Scheduling it often is only recommended when cache consists of
    // numerous frequently added short-lived items and not running it often enough will result in high memory usage.
    public static readonly TimeSpan CacheItemsEvictionInterval = TimeSpan.TryParse(GetVar("FASTCACHE_EVICTION_INTERVAL"), out var parsed)
            ? parsed : TimeSpan.FromSeconds(3);

    private static string? GetVar(string key) => Environment.GetEnvironmentVariable(key);
}
