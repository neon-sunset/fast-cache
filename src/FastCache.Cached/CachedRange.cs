namespace FastCache.Collections;

public static partial class CachedRange
{
    public static void Save<K, V>((K, V)[] range, TimeSpan expiration) where K : notnull =>
        Save(range.AsMemory(), expiration);

    public static void Save<K, V>(Memory<(K, V)> range, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<(K, V)>)range, expiration);

    public static void Save<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        if (range.Length <= 0)
        {
            return;
        }

        throw new NotImplementedException();
    }

    public static void Save<K, V>(K[] keys, V[] values, TimeSpan expiration) where K : notnull =>
        Save(keys.AsMemory(), values.AsMemory(), expiration);

    public static void Save<K, V>(Memory<K> keys, Memory<V> values, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<K>)keys, (ReadOnlyMemory<V>)values, expiration);

    public static void Save<K, V>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration) where K : notnull
    {
        ValidateLength(keys.Length, values.Length);

        throw new NotImplementedException();
    }

    public static void Save<K, V>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        throw new NotImplementedException();
    }

    public static void Save<K, V>(IEnumerable<K> keys, IEnumerable<V> values, TimeSpan expiration) where K : notnull
    {
        throw new NotImplementedException();
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
