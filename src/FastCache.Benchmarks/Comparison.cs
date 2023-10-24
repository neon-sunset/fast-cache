using CacheManager.Core;
using FastCache.Extensions;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;

namespace FastCache.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 5, exportCombinedDisassemblyReport: true)]
public class Comparison
{
    [Params("A", "abcd", "long ass string with букви кирилицею AND UPPERCASE")]
    public string ItemKey = default!;
    public string ItemValue = "item value";

    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    // CacheManager documentation by default suggests using WithMicrosoftMemoryCacheHandle() which has terrible performance
    // when compared to WithDictionaryHandle(). However, we won't be too hard on it and will give it the best chance.
    private readonly ICacheManager<string> _cacheManager = CacheFactory
        .Build<string>(p => p
        .WithDictionaryHandle()
        .WithExpiration(CacheManager.Core.ExpirationMode.Absolute, TimeSpan.FromMinutes(60)));

    private readonly IAppCache _lazyCache = new CachingService();
    private readonly FusionCache _fusionCache = new (new FusionCacheOptions());

    [GlobalSetup]
    public void Initialize()
    {
        ItemValue.Cache(ItemKey, TimeSpan.FromMinutes(60));
        _memoryCache.Set(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
        _cacheManager.AddOrUpdate(ItemKey, ItemValue, _ => ItemValue);
        _lazyCache.Add(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
        _fusionCache.Set(ItemKey, ItemValue, options => options.SetDuration(TimeSpan.FromMinutes(60)));

        Services.CacheManager.SuspendEviction<string, string>();
    }

    [Benchmark(Baseline = true)]
    public string TryGetCached()
    {
        return Cached<string>.TryGet(ItemKey, out var cached) ? cached : Unreachable<string>();
    }

    [Benchmark]
    public string TryGetMemoryCache()
    {
        return _memoryCache.TryGetValue(ItemKey, out var result) && result is string value
            ? value
            : Unreachable<string>();
    }

    [Benchmark]
    public string TryGetCacheManager()
    {
        return _cacheManager.Get(ItemKey) ?? Unreachable<string>();
    }

    [Benchmark]
    public string TryGetLazyCache()
    {
        return _lazyCache.TryGetValue<string>(ItemKey, out var value) ? value : Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFusionCache()
    {
        return _fusionCache.GetOrDefault<string>(ItemKey) ?? Unreachable<string>();
    }

    [Benchmark]
    public void UpdateCached()
    {
        ItemValue.Cache(ItemKey, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void UpdateMemoryCache()
    {
        _memoryCache.Set(ItemKey, ItemValue, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void UpdateCacheManager()
    {
        _cacheManager.Put(ItemKey, ItemValue);
    }

    [Benchmark]
    public void UpdateLazyCache()
    {
        _lazyCache.Add(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void UpdateFusionCache()
    {
        _fusionCache.Set(ItemKey, ItemValue, options => options.SetDuration(TimeSpan.FromMinutes(60)));
    }
}
