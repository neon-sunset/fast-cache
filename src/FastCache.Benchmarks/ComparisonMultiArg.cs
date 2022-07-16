using FastCache.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

[ShortRunJob]
[DisassemblyDiagnoser(maxDepth: 5, exportCombinedDisassemblyReport: true)]
public class ComparisonMultiArg
{
    private static readonly TimeSpan Expiration = TimeSpan.FromHours(10);

    public string ItemKey = "lmao";
    public string ItemValue = "item value";

    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    [GlobalSetup]
    public void Initialize()
    {
        ItemValue.Cache(ItemKey, ItemKey, Expiration);
        ItemValue.Cache(ItemKey, ItemKey, ItemKey, ItemKey, Expiration);

        _memoryCache.Set((ItemKey, ItemKey), ItemValue, Expiration);
        _memoryCache.Set((ItemKey, ItemKey, ItemKey, ItemKey), ItemValue, Expiration);
    }

    [Benchmark(Baseline = true)]
    public string TryGetCachedTwo()
    {
        if (Cached<string>.TryGet(ItemKey, ItemKey, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetCachedFour()
    {
        if (Cached<string>.TryGet(ItemKey, ItemKey, ItemKey, ItemKey, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetMemoryCacheTwo()
    {
        var found = _memoryCache.TryGetValue((ItemKey, ItemKey), out var cacheItem);
        if (found && cacheItem is string value)
        {
            return value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetMemoryCacheFour()
    {
        var found = _memoryCache.TryGetValue((ItemKey, ItemKey, ItemKey, ItemKey), out var cacheItem);
        if (found && cacheItem is string value)
        {
            return value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public void UpdateCachedTwo()
    {
        ItemValue.Cache(ItemKey, ItemKey, Expiration);
    }

    [Benchmark]
    public void UpdateCachedFour()
    {
        ItemValue.Cache(ItemKey, ItemKey, ItemKey, ItemKey, Expiration);
    }

    [Benchmark]
    public void UpdateMemoryCacheTwo()
    {
        _memoryCache.Set((ItemKey, ItemKey), ItemValue, Expiration);
    }

    [Benchmark]
    public void UpdateMemoryCacheFour()
    {
        _memoryCache.Set((ItemKey, ItemKey, ItemKey, ItemKey), ItemValue, Expiration);
    }
}
