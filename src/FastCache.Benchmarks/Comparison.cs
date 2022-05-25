using System.Runtime.Caching;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CacheManager.Core;
using FastCache.Extensions;
using LazyCache;

namespace FastCache.Benchmarks
{
    [MemoryDiagnoser]
    // [DisassemblyDiagnoser(maxDepth: 5, exportHtml: true)]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    // [SimpleJob(RuntimeMoniker.Net60)]
    // [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class Comparison
    {
        private const string ItemKey = "singleKey";
        private const string ItemValue = "single value string";

        private readonly MemoryCache _memoryCache = MemoryCache.Default;

        private readonly ICacheManager<string> _cacheManager = CacheFactory.Build<string>(p => p
            .WithMicrosoftMemoryCacheHandle()
            .WithExpiration(CacheManager.Core.ExpirationMode.Absolute, TimeSpan.FromMinutes(60)));

        private readonly IAppCache _lazyCache = new CachingService()
        {
            DefaultCachePolicy = new CacheDefaults() { DefaultCacheDurationSeconds = 3600 }
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
            _cacheManager.TryUpdate(ItemKey, static _ => ItemValue, out _);
        }

        [Benchmark]
        public void UpdateLazyCache()
        {
            _lazyCache.Add(ItemKey, ItemValue, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60));
        }
    }
}
