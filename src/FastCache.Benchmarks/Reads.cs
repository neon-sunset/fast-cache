using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Collections;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[DisassemblyDiagnoser(maxDepth: 5, exportCombinedDisassemblyReport: true)]
public class Reads
{
    private const string ItemValue = "1337";

    private static readonly (string, string)[] Range = Enumerable.Range(0, 10_000_000).Select(i => (i.ToString(), ItemValue)).ToArray();

    [GlobalSetup]
    public void GlobalSetup()
    {
        Services.CacheManager.SuspendEviction<string, string>();
        CachedRange.Save(Range, TimeSpan.FromHours(3));

        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);
        Seed(kvp => _memoryCache.Set(kvp.Item1, kvp.Item2, expiration));
    }

    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    [Params(1000, 100_000, 1_000_000, 10_000_000)]
    public int Length;

    [Benchmark(Baseline = true)]
    public string TryGetMT()
    {
        var keys = Range.AsMemory()[..Length];
        var sliceLength = keys.Length / Environment.ProcessorCount;
        var value = string.Empty;

        Parallel.For(0, Environment.ProcessorCount, i => ReadSlice(i));

        void ReadSlice(int i)
        {
            var ret = string.Empty;

            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            foreach (var (key, _) in keys.Span[start..end])
            {
                if (Cached<string>.TryGet(key, out var cached))
                {
                    ret = cached.Value;
                }
                else
                {
                    ret = Unreachable<string>();
                }
            }

            value = ret;
        }

        return value;
    }

    [Benchmark]
    public string TryGetST()
    {
        var value = string.Empty;

        foreach (var (key, _) in Range.AsSpan()[..Length])
        {
            if (Cached<string>.TryGet(key, out var cached))
            {
                value = cached.Value;
            }
            else
            {
                value = Unreachable<string>();
            }
        }

        return value;
    }

    [Benchmark]
    public string MemoryCacheMT()
    {
        var keys = Range.AsMemory()[..Length];
        var sliceLength = keys.Length / Environment.ProcessorCount;
        var value = string.Empty;

        Parallel.For(0, Environment.ProcessorCount, i => ReadSlice(i));

        void ReadSlice(int i)
        {
            var ret = string.Empty;

            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            foreach (var (key, _) in keys.Span[start..end])
            {
                var found = _memoryCache.TryGetValue(key, out var cacheItem);
                if (found && cacheItem is string value)
                {
                    ret = value;
                }
                else
                {
                    ret = Unreachable<string>();
                }
            }

            value = ret;
        }

        return value;
    }

    [Benchmark]
    public string MemoryCacheST()
    {
        var value = string.Empty;

        foreach (var (key, _) in Range.AsSpan()[..Length])
        {
            var found = _memoryCache.TryGetValue(key, out var cacheItem);
            if (found && cacheItem is string stringValue)
            {
                value = stringValue;
            }
            else
            {
                value = Unreachable<string>();
            }
        }

        return value;
    }

    private static void Seed(Action<(string, string)> action)
    {
        foreach (var kvp in Range)
        {
            action(kvp);
        }
    }
}
