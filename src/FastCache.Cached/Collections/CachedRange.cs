using FastCache.Helpers;

namespace FastCache.Collections;

public static partial class CachedRange<V>
{
    public static void Save<K>((K, V)[] range, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<(K, V)>)range, expiration);

    public static void Save<K>(Memory<(K, V)> range, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<(K, V)>)range, expiration);

    public static void Save<K>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var length = range.Length;
        if (length <= 0)
        {
            return;
        }

        var parallelism = GetParallelism((uint)length);
        if (parallelism > 1)
        {
            SaveMultithreaded(range, expiration, (int)parallelism);
        }
        else
        {
            SaveSinglethreaded(range, expiration);
        }
    }

    public static void Save<K>(K[] keys, V[] values, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<K>)keys, (ReadOnlyMemory<V>)values, expiration);

    public static void Save<K>(Memory<K> keys, Memory<V> values, TimeSpan expiration) where K : notnull =>
        Save((ReadOnlyMemory<K>)keys, (ReadOnlyMemory<V>)values, expiration);

    public static void Save<K>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration) where K : notnull
    {
        var length = keys.Length;
        if (length < 0 || length != values.Length)
        {
            ThrowHelpers.IncorrectSaveLength(length, values.Length);
        }

        var parallelism = GetParallelism((uint)length);
        if (parallelism > 1)
        {
            SaveMultithreaded(keys, values, expiration, (int)parallelism);
        }
        else
        {
            SaveSinglethreaded(keys, values, expiration);
        }
    }

    public static void Save<K>(IList<(K, V)> range, TimeSpan expiration)
        where K : notnull
    {
        var length = range.Count;
        if (length <= 0)
        {
            return;
        }

        if (range is (K, V)[] array)
        {
            Save(array, expiration);
            return;
        }

        var parallelism = GetParallelism((uint)length);
        if (parallelism > 1)
        {
            if (range is List<(K, V)> list)
            {
                SaveListMultithreaded<K, List<(K, V)>>(list, expiration, (int)parallelism);
            }
            else
            {
                SaveListMultithreaded<K, IList<(K, V)>>(range, expiration, (int)parallelism);
            }
        }
        else if (range is List<(K, V)> list)
        {
            SaveListSinglethreaded<K, List<(K, V)>>(list, expiration);
        }
        else
        {
            SaveListSinglethreaded<K, IList<(K, V)>>(range, expiration);
        }
    }

    public static void Save<K>(IEnumerable<(K, V)> range, TimeSpan expiration)
        where K : notnull
    {
        if (range is (K, V)[] array)
        {
            Save(array, expiration);
        }
        else if (range is IList<(K, V)> genericList)
        {
            Save(genericList, expiration);
        }
#if NET6_0_OR_GREATER
        else if (range.TryGetNonEnumeratedCount(out var length) && GetParallelism((uint)length) > 1)
        {
            SaveEnumerableMultithreaded(range, expiration);
        }
#endif
        else
        {
            SaveEnumerableSinglethreaded(range, expiration);
        }
    }

    public static void Remove<K>(K[] keys) where K : notnull =>
        Remove((ReadOnlyMemory<K>)keys);

    public static void Remove<K>(Memory<K> keys) where K : notnull =>
        Remove((ReadOnlyMemory<K>)keys);

    public static void Remove<K>(ReadOnlyMemory<K> keys) where K : notnull
    {
        var parallelism = GetParallelism((uint)keys.Length);
        if (parallelism > 1)
        {
            RemoveMultithreaded(keys, (int)parallelism);
        }
        else
        {
            RemoveSlice(keys.Span);
        }
    }

    public static void Remove<K>(IEnumerable<K> keys) where K : notnull
    {
        if (keys is K[] array)
        {
            Remove((ReadOnlyMemory<K>)array);
        }
#if NET6_0_OR_GREATER
        else if (keys.TryGetNonEnumeratedCount(out var count) && GetParallelism((uint)count) > 1)
        {
            keys.AsParallel()
                .ForAll(static key => CacheStaticHolder<K, V>.Store.TryRemove(key, out _));
        }
#endif
        else
        {
            foreach (var key in keys)
            {
                CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
            }
        }
    }
}
