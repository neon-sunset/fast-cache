using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CacheManager.Core;
using FastCache.Extensions;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(maxDepth: 5, exportHtml: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class Comparison
    {
        enum EnumKey { Default, Custom }

        private const string ItemKey = "item key value";
        private const string ItemValue = "single value string";

        private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

        private readonly ICacheManager<string> _cacheManager = CacheFactory
            .Build<string>(p => p
            .WithMicrosoftMemoryCacheHandle()
            .WithExpiration(CacheManager.Core.ExpirationMode.Absolute, TimeSpan.FromMinutes(60)));

        private readonly IAppCache _lazyCache = new CachingService()
        {
            DefaultCachePolicy = new CacheDefaults() { DefaultCacheDurationSeconds = 3600 }
        };

        [GlobalSetup]
        public void Initialize()
        {
            Services.CacheManager.SuspendEviction<string, string>();
        }

        [Benchmark(Baseline = true)]
        public string TryGetCached()
        {
            if (Cached<string>.TryGet(ItemKey, out var cached))
            {
                return cached.Value;
            }

            return cached.Save(ItemValue, TimeSpan.FromMinutes(60));
        }

        [Benchmark]
        public string TryGetMemoryCache()
        {
            if (_memoryCache.Get(ItemKey) is string value and not null)
            {
                return value;
            }

            var newItem = ItemValue;

            _memoryCache.Set(ItemKey, newItem, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));

            return newItem;
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

            _lazyCache.Add(ItemKey, saved, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));

            return saved;
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
    }
}
