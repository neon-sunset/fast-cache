using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CacheManager.Core;
using FastCache.Extensions;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 10, exportCombinedDisassemblyReport: true, printSource: true)]
[SimpleJob(RuntimeMoniker.Net60)]
public class Comparison
{
    enum EnumKey { Default, Custom }

    private const string ItemKey = "item key value";
    private const string ItemValue = "single value string";

    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    // CacheManager documentation by default suggest using WithMicrosoftMemoryCacheHandle() which has terrible performance
    // when compared to WithDictionaryHandle(). However, we won't be too hard on it and will give it the best chance.
    private readonly ICacheManager<string> _cacheManager = CacheFactory
        .Build<string>(p => p
        .WithDictionaryHandle()
        .WithExpiration(CacheManager.Core.ExpirationMode.Absolute, TimeSpan.FromMinutes(60)));

    private readonly IAppCache _lazyCache = new CachingService()
    {
        DefaultCachePolicy = new CacheDefaults() { DefaultCacheDurationSeconds = 3600 }
    };

    [GlobalSetup]
    public void Initialize()
    {
        ItemValue.Cache(ItemKey, TimeSpan.FromMinutes(60));
        _memoryCache.Set(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
        _cacheManager.AddOrUpdate(ItemKey, ItemValue, static _ => ItemValue);
        _lazyCache.Add(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));

        Services.CacheManager.SuspendEviction<string, string>();
    }

    [Benchmark(Baseline = true)]
    public string TryGetCached()
    {
        if (Cached<string>.TryGet(ItemKey, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetMemoryCache()
    {
        var found = _memoryCache.TryGetValue(ItemKey, out var cacheItem);
        if (found && cacheItem is string value)
        {
            return value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetCacheManager()
    {
        var value = _cacheManager.Get(ItemKey);
        if (value is not null)
        {
            return value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetLazyCache()
    {
        if (_lazyCache.TryGetValue<string>(ItemKey, out var value))
        {
            return value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public void UpdateCached()
    {
        ItemValue.Cache(ItemKey, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void UpdateMemoryCache()
    {
        _memoryCache.Set(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void UpdateCacheManager()
    {
        _cacheManager.AddOrUpdate(ItemKey, ItemValue, static _ => ItemValue);
    }

    [Benchmark]
    public void UpdateLazyCache()
    {
        _lazyCache.Add(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
    }

    [DoesNotReturn]
    private static string Unreachable<T>() => throw new InvalidOperationException();
}
