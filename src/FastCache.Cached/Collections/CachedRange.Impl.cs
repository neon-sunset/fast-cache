using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;

namespace FastCache.Collections;

public static partial class CachedRange
{
    private readonly record struct ListSlice<T, TList> where TList : IList<T>
    {
        public readonly TList List;
        public readonly int Start;
        public readonly int End;

        public ListSlice(TList list, int start, int end)
        {
            List = list;
            Start = start;
            End = end;
        }
    }

    internal static void SaveSinglethreaded<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

        SaveSlice(range.Span, timestamp);
    }

    private static void SaveSinglethreaded<K, V>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration) where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

        SaveSlice(keys.Span, values.Span, timestamp);
    }

    private static void SaveMultithreaded<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration, int parallelism) where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

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

        foreach (var (key, value) in range.Span[^remainderLength..^0])
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void SaveMultithreaded<K, V>(ReadOnlyMemory<K> keys, ReadOnlyMemory<V> values, TimeSpan expiration, int parallelism) where K : notnull
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

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

        var remainderKeys = keys.Span[^remainderLength..^0];
        var remainderValues = values.Span[^remainderLength..^0];

        for (int i = 0; i < remainderKeys.Length; i++)
        {
            var key = remainderKeys[i];

            CacheStaticHolder<K, V>.Store[key] = new(remainderValues[i], timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void SaveListSinglethreaded<K, V, TList>(TList range, TimeSpan expiration)
        where K : notnull
        where TList : IList<(K, V)>
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

        SaveListSlice<K, V, TList>(new(range, 0, range.Count), timestamp);
    }

    private static void SaveListMultithreaded<K, V, TList>(TList range, TimeSpan expiration, int parallelism)
        where K : notnull
        where TList : IList<(K, V)>
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

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

            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void SaveEnumerableSinglethreaded<K, V, TEnumerable>(TEnumerable range, TimeSpan expiration)
        where K : notnull
        where TEnumerable : IEnumerable<(K, V)>
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void SaveEnumerableMultithreaded<K, V, TEnumerable>(TEnumerable range, TimeSpan expiration)
        where K : notnull
        where TEnumerable : IEnumerable<(K Key, V Value)>
    {
        var timestamp = GetAndReportTimestamp<K, V>(expiration);

        range
            .AsParallel()
            .AsUnordered()
            .ForAll(SaveEntry);

        void SaveEntry((K Key, V Value) kvp)
        {
            var (key, value) = (kvp.Key, kvp.Value);

            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }
    }

    private static void RemoveMultithreaded<K, V>(ReadOnlyMemory<K> keys, int parallelism) where K : notnull
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

        Parallel.ForEach(memorySlices, static slice => RemoveSlice<K, V>(slice.Span));
    }

    private static void RemoveSlice<K, V>(ReadOnlySpan<K> slice) where K : notnull
    {
        foreach (var key in slice)
        {
            CacheStaticHolder<K, V>.Store.TryRemove(key, out _);
        }
    }

    private static void SaveSlice<K, V>(ReadOnlySpan<(K key, V value)> slice, long timestamp) where K : notnull
    {
        foreach (var (key, value) in slice)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K, V>(slice.Length);

        for (var i = 0; i < quickListLimit; i++)
        {
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(slice[i].key, timestamp);
        }
    }

    private static void SaveSlice<K, V>(ReadOnlySpan<K> keys, ReadOnlySpan<V> values, long timestamp) where K : notnull
    {
        for (var i = 0; i < keys.Length; i++)
        {
            CacheStaticHolder<K, V>.Store[keys[i]] = new(values[i], timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K, V>(keys.Length);

        for (var i = 0; i < quickListLimit; i++)
        {
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(keys[i], timestamp);
        }
    }

    private static void SaveListSlice<K, V, TList>(ListSlice<(K, V), TList> slice, long timestamp)
        where K : notnull
        where TList : IList<(K Key, V Value)>
    {
        var list = slice.List;
        var start = slice.Start;
        var end = slice.End;

        for (var i = start; i < end; i++)
        {
            var (key, value) = list[i];

            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
        }

        var quickListLimit = GetQuickListInsertLength<K, V>(end - start) + start;

        for (var i = start; i < quickListLimit; i++)
        {
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(list[i].Key, timestamp);
        }
    }

    private static long GetAndReportTimestamp<K, V>(TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        return timestamp;
    }

    private static int GetQuickListInsertLength<K, V>(int sliceLength) where K : notnull
    {
        return (int)Math.Min(CacheStaticHolder<K, V>.QuickList.FreeSpace, (uint)sliceLength);
    }

    private static uint GetParallelism(uint length)
    {
        return Math.Max(Math.Min(length / Constants.ParallelSaveMinBatchSize, (uint)Environment.ProcessorCount), 1);
    }

    [DoesNotReturn]
    private static void RangeMismatch(int keys, int values)
    {
        throw new ArgumentOutOfRangeException($"Cannot perform 'Save()' for provided ranges - length mismatch, keys: {keys}, values: {values}.");
    }
}
