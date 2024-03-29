using FastCache.Collections;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 5, exportCombinedDisassemblyReport: true)]
public class Reads
{
    private const string ItemValue = "1337";

    private static (string, string)[] Range = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Range = (0..Length).Select(i => ($"{i}", ItemValue)).ToArray();

        Services.CacheManager.SuspendEviction<string, string>();
        CachedRange<string>.Save(Range, TimeSpan.FromHours(3));

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

        Parallel.For(0, Environment.ProcessorCount, ReadSlice);

        void ReadSlice(int i)
        {
            var ret = string.Empty;

            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            foreach (var (key, _) in keys.Span[start..end])
            {
                ret = Cached<string>.TryGet(key, out var cached) ? cached : Unreachable<string>();
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
            value = Cached<string>.TryGet(key, out var cached) ? cached : Unreachable<string>();
        }

        return value;
    }

    [Benchmark]
    public string MemoryCacheMT()
    {
        var keys = Range.AsMemory()[..Length];
        var sliceLength = keys.Length / Environment.ProcessorCount;
        var value = string.Empty;

        Parallel.For(0, Environment.ProcessorCount, ReadSlice);

        void ReadSlice(int i)
        {
            var ret = string.Empty;

            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            foreach (var (key, _) in keys.Span[start..end])
            {
                ret = _memoryCache.TryGetValue(key, out var cacheItem)
                    && cacheItem is string value ? value : Unreachable<string>();
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
            value = _memoryCache.TryGetValue(key, out var cacheItem) && cacheItem is string stringValue
                ? stringValue : Unreachable<string>();
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
