using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;

namespace FastCache.Collections;

public static partial class CachedRange
{
    private static void SaveSinglethreaded<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        ProcessSlice(range.Span, timestamp);
    }

    private static void SaveSinglethreadedEnumerable<K, V>(IEnumerable<(K, V)> range, TimeSpan expiration, int parallelism) where K : notnull
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        foreach (var (key, value) in range)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(key, timestamp);
        }

        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);
    }

    private static void SaveMultithreaded<K, V>(ReadOnlyMemory<(K, V)> range, TimeSpan expiration, int parallelism) where K : notnull
    {
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

    private static void SaveMultithreadedEnumerable<K, V>(IEnumerable<(K, V)> range, TimeSpan expiration, int parallelism) where K : notnull
    {
        throw new NotImplementedException();
    }

    private static void ProcessSlice<K, V>(ReadOnlySpan<(K, V)> slice, long timestamp) where K : notnull
    {
        foreach (var (key, value) in slice)
        {
            CacheStaticHolder<K, V>.Store[key] = new(value, timestamp);
        }

        var quickListInsertLength = (int)Math.Min(CacheStaticHolder<K, V>.QuickList.FreeSpace, (uint)slice.Length);
        for (var i = 0; i < quickListInsertLength; i++)
        {
            CacheStaticHolder<K, V>.QuickList.OverwritingNonAtomicAdd(slice[i].Item1, timestamp);
        }
    }

    private static uint CalculateParalellism(uint length)
    {
        return Math.Max(Math.Min(length / Constants.ParallelSaveMinBatchSize, (uint)Environment.ProcessorCount), 1);
    }

    [DoesNotReturn]
    private static void ValidateLength(int keys, int values)
    {
        throw new NotImplementedException();
    }
}
