using BenchmarkDotNet.Attributes;
using CacheManager.Core;
using FastCache.Extensions;
using LazyCache;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
public class Comparison
{
    private const string ItemKey = "singleKey";
    private const string ItemValue = "single value string";

    private static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);

    private static readonly DateTimeOffset InOneHour = DateTimeOffset.UtcNow + OneHour;

    private readonly ICacheManager<string> _cacheManager = CacheFactory.Build<string>(p => p.WithMicrosoftMemoryCacheHandle());

    private readonly IAppCache _lazyCache = new CachingService()
    {
        DefaultCachePolicy = new CacheDefaults() { DefaultCacheDurationSeconds = (int)OneHour.TotalSeconds }
    };

    [GlobalSetup]
    public void Initialize()
    {
        Services.CacheManager.SuspendEviction<string>();
    }

    [Benchmark(Baseline = true)]
    public string TryGetCached()
    {
        if (Cached<string>.TryGet(ItemKey, out var cached))
        {
            return cached.Value;
        }

        return cached.Save(ItemValue, OneHour);
    }

    [Benchmark]
    public string TryGetCacheManager()
    {
        return _cacheManager.GetOrAdd(ItemKey, ItemValue);
    }

    [Benchmark]
    public string TryGetLazyCache()
    {
        if (_lazyCache.TryGetValue<string>(ItemKey, out var value))
        {
            return value;
        }

        var saved = ItemValue;

        _lazyCache.Add(ItemKey, saved, InOneHour);

        return saved;
    }

    [Benchmark]
    public void UpdateCached()
    {
        ItemValue.Cache(ItemKey, OneHour);
    }

    [Benchmark]
    public void UpdateCacheManager()
    {
        _cacheManager.TryUpdate(ItemKey, static _ => ItemValue, out _);
    }

    [Benchmark]
    public void UpdateLazyCache()
    {
        _lazyCache.Add(ItemKey, ItemValue, InOneHour);
    }
}
