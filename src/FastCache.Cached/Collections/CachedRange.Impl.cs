using FastCache.Helpers;

namespace FastCache.Collections;

public static partial class CachedRange<V>
{
    private record struct ListSlice<T, TList>(TList List, int Start, int End)
        where TList : IList<T>;

    internal static void SaveSinglethreaded<K>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K>(expiration);

        SaveSlice(range.Span, timestamp);
    }

    private static void SaveSinglethreaded<K>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration)
        where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K>(expiration);

        SaveSlice(keys.Span, values.Span, timestamp);
    }

    private static void SaveMultithreaded<K>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration, int parallelism)
        where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K>(expiration);

        var sliceLength = range.Length / parallelism;
        var remainderLength = range.Length % parallelism;

        var memorySlices = new (ReadOnlyMemory<(K, V)> Value, long Timestamp)[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            memorySlices[i] = (range[start..end], timestamp);
        }

        Parallel.ForEach(memorySlices, static slice => SaveSlice(slice.Value.Span, slice.Timestamp));

        // Handle remainder
        SaveSlice(range.Span[^remainderLength..], timestamp);
    }

    private static void SaveMultithreaded<K>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration, int parallelism)
        where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K>(expiration);

        var sliceLength = keys.Length / parallelism;
        var remainderLength = keys.Length % parallelism;

        var memorySlices = new (ReadOnlyMemory<K> Keys, ReadOnlyMemory<V> Values, long Timestamp)[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            memorySlices[i] = (keys[start..end], values[start..end], timestamp);
        }

        Parallel.ForEach(memorySlices, static slice => SaveSlice(slice.Keys.Span, slice.Values.Span, slice.Timestamp));

        // Handle remainder
        SaveSlice(keys.Span[^remainderLength..], values.Span[^remainderLength..], timestamp);
    }

    private static void SaveListSinglethreaded<K, TList>(TList range, TimeSpan expiration)
        where K : notnull
        where TList : IList<(K, V)>
    {
        var timestamp = GetAndReportTimestamp<K>(expiration);

        SaveListSlice<K, TList>(new(range, 0, range.Count), timestamp);
    }

    private static void SaveListMultithreaded<K, TList>(TList range, TimeSpan expiration, int parallelism)
        where K : notnull
        where TList : IList<(K, V)>
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);
        var timestamp = GetAndReportTimestamp<K>(expiration);

        var sliceLength = range.Count / parallelism;
        var remainderLength = range.Count % parallelism;

        var listSlices = new (ListSlice<(K, V), TList> Value, long Timestamp)[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            listSlices[i] = (new(range, start, end), timestamp);
        }

        Parallel.ForEach(listSlices, static slice => SaveListSlice(slice.Value, slice.Timestamp));

        for (int i = range.Count - remainderLength; i < range.Count; i++)
        {
            var (key, value) = range[i];

            store[key] = new(value, timestamp);
            quickList.OverwritingAdd(key, timestamp);
        }
    }

    private static void SaveEnumerableSinglethreaded<K>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);
        var timestamp = GetAndReportTimestamp<K>(expiration);

        foreach (var (key, value) in range)
        {
            store[key] = new(value, timestamp);
        }

        quickList.PullFromCacheStore();
    }

    private static void SaveEnumerableMultithreaded<K>(IEnumerable<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);
        var timestamp = GetAndReportTimestamp<K>(expiration);

        range
            .AsParallel()
            .AsUnordered()
            .ForAll(kvp => store[kvp.Item1] = new(kvp.Item2, timestamp));

        quickList.PullFromCacheStore();
    }

    private static void RemoveMultithreaded<K>(ReadOnlyMemory<K> keys, int parallelism) where K : notnull
    {
        var sliceLength = keys.Length / parallelism;
        var remainderLength = keys.Length % parallelism;

        var memorySlices = new ReadOnlyMemory<K>[parallelism];

        for (int i = 0; i < parallelism; i++)
        {
            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            memorySlices[i] = keys[start..end];
        }

        Parallel.ForEach(memorySlices, static slice => RemoveSlice(slice.Span));

        // Handle remainder
        RemoveSlice(keys.Span[^remainderLength..]);
    }

    private static void RemoveSlice<K>(ReadOnlySpan<K> slice) where K : notnull
    {
        var store = CacheStaticHolder<K, V>.Store;
        foreach (var key in slice)
        {
            store.TryRemove(key, out _);
        }
    }

    private static void SaveSlice<K>(ReadOnlySpan<(K key, V value)> slice, long timestamp) where K : notnull
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);

        foreach (var (key, value) in slice)
        {
            store[key] = new(value, timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K>(slice.Length);
        for (var i = 0; i < quickListLimit; i++)
        {
            quickList.OverwritingAdd(slice[i].key, timestamp);
        }
    }

    private static void SaveSlice<K>(ReadOnlySpan<K> keys, ReadOnlySpan<V> values, long timestamp) where K : notnull
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);

        for (var i = 0; i < keys.Length; i++)
        {
            store[keys[i]] = new(values[i], timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K>(keys.Length);
        for (var i = 0; i < quickListLimit; i++)
        {
            quickList.OverwritingAdd(keys[i], timestamp);
        }
    }

    private static void SaveListSlice<K, TList>(ListSlice<(K, V), TList> slice, long timestamp)
        where K : notnull
        where TList : IList<(K Key, V Value)>
    {
        var (store, quickList) = (
            CacheStaticHolder<K, V>.Store,
            CacheStaticHolder<K, V>.QuickList);
        var (list, start, end) = slice;

        // Surprisingly, on plain lists the performance is within 1.1x of array/span impl.
        // Perhaps devirtualization via generics and TieredPGO is all that we need
        for (var i = start; i < end; i++)
        {
            var (key, value) = list[i];

            store[key] = new(value, timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K>(end - start) + start;

        for (var i = start; i < quickListLimit; i++)
        {
            quickList.OverwritingAdd(list[i].Key, timestamp);
        }
    }

    private static long GetAndReportTimestamp<K>(TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        return timestamp;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetQuickListInsertLength<K>(int sliceLength) where K : notnull
    {
        return (int)Math.Min(CacheStaticHolder<K, V>.QuickList.FreeSpace, (uint)sliceLength);
    }

    private static uint GetParallelism(uint length)
    {
        return Math.Max(Math.Min(length / Constants.ParallelSaveMinBatchSize, (uint)Environment.ProcessorCount), 1);
    }
}
