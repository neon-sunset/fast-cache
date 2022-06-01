using FastCache.Helpers;

namespace FastCache.Collections;

public static class CachedRange<V>
{
    public static void Save<K>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.s_store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.s_quickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.s_evictionJob.ReportExpiration(milliseconds);
    }

    public static void Save<K>(ReadOnlySpan<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        if (range.Length <= 0)
        {
            return;
        }

        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.s_store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.s_quickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.s_evictionJob.ReportExpiration(milliseconds);
    }

    public static void Remove<K>(IEnumerable<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.s_store.TryRemove(key, out _);
        }
    }

    public static void Remove<K>(ReadOnlySpan<K> keys) where K : notnull
    {
        foreach (var key in keys)
        {
            CacheStaticHolder<K, V>.s_store.TryRemove(key, out _);
        }
    }
}
