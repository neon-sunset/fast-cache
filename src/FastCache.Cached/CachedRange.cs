using System.Collections.Concurrent;
using FastCache.Helpers;

namespace FastCache.Collections;

public static class CachedRange<V>
{
    public static void Save<K>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);
    }

    public static void Save<K>(Span<(K, V)> range, TimeSpan expiration) where K : notnull => Save((ReadOnlySpan<(K, V)>)range, expiration);

    public static void Save<K>(ReadOnlySpan<(K, V)> range, TimeSpan expiration) where K : notnull
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

    public static void SaveMultithreaded<K>(Memory<(K, V)> range, TimeSpan expiration, int parallelism = -1) where K : notnull =>
        SaveMultithreaded((ReadOnlyMemory<(K, V)>)range, expiration, parallelism);

    public static void SaveMultithreaded<K>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration, int parallelism = -1) where K : notnull
    {
        if (parallelism is -1)
        {
            parallelism = Environment.ProcessorCount;
        }

        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        var store = CacheStaticHolder<K, V>.Store;
        var quickList = CacheStaticHolder<K, V>.QuickList;

        var sliceLength = range.Length / parallelism;
        var remainderLength = range.Length % parallelism;

        var memorySlices = new (ReadOnlyMemory<(K, V)>, long)[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            memorySlices[i] = (range[start..end], timestamp);
        }

        Parallel.ForEach(memorySlices, static sliceState => ProcessSlice(sliceState.Item1.Span, sliceState.Item2));

        foreach (var (key, value) in range.Span[^remainderLength..^0])
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void ProcessSlice<K>(ReadOnlySpan<(K, V)> slice, long timestamp) where K : notnull
    {
        foreach (var (key, value) in slice)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    public static void Remove<K>(IEnumerable<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
        }
    }

    public static void Remove<K>(ReadOnlySpan<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
        }
    }
}
