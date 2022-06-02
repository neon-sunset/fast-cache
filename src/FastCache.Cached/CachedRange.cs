using FastCache.Helpers;

namespace FastCache.Collections;

public static class CachedRange
{
    public static void Save<K, V>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);
    }

    public static void Save<K, V>(Span<(K, V)> range, TimeSpan expiration) where K : notnull => Save((ReadOnlySpan<(K, V)>)range, expiration);

    public static void Save<K, V>(ReadOnlySpan<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        if (range.Length <= 0)
        {
            return;
        }

        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);
    }

    public static void SaveMultithreaded<K, V>(Memory<(K, V)> range, TimeSpan expiration, int parallelism = -1) where K : notnull =>
        SaveMultithreaded((ReadOnlyMemory<(K, V)>)range, expiration, parallelism);

    public static void SaveMultithreaded<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration, int parallelism = -1) where K : notnull
    {
        if (parallelism <= -1)
        {
            parallelism = Environment.ProcessorCount;
        }

        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        var sliceLength = range.Length / parallelism;
        var remainderLength = range.Length % parallelism;

        var memorySlices = new (ReadOnlyMemory<(K, V)> Value, long Timestamp)[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            memorySlices[i] = (range[start..end], timestamp);
        }

        Parallel.ForEach(memorySlices, static slice => ProcessSlice(slice.Value.Span, slice.Timestamp));

        foreach (var (key, value) in range.Span[^remainderLength..^0])
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void ProcessSlice<K, V>(ReadOnlySpan<(K, V)> slice, long timestamp) where K : notnull
    {
        foreach (var (key, value) in slice)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    public static void Remove<K, V>(IEnumerable<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
        }
    }

    public static void Remove<K, V>(ReadOnlySpan<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
        }
    }
}
