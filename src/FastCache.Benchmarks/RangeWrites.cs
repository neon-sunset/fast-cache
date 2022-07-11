using CacheManager.Core;
using FastCache.Collections;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

// WARNING: Takes up to 4GB of RAM to run 10M option
[ShortRunJob]
[MemoryDiagnoser]
public class RangeWrites
{
    private const string ItemValue = "1337";

    private static (string, string)[] Range = default!;

    [Params(1000, 100_000, 1_000_000, 10_000_000)]
    public int Length;

    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    private readonly ICacheManager<string> _cacheManager = CacheFactory
        .Build<string>(p => p
        .WithDictionaryHandle()
        .WithExpiration(CacheManager.Core.ExpirationMode.Absolute, TimeSpan.FromHours(3)));

    private readonly IAppCache _lazyCache = new CachingService();

    [GlobalSetup]
    public void GlobalSetup()
    {
        Services.CacheManager.SuspendEviction<string, string>();
        Range = Enumerable.Range(0, Length).Select(i => (i.ToString(), ItemValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public void Save()
    {
        CachedRange<string>.Save(Range, TimeSpan.FromHours(3));
    }

    [Benchmark]
    public void SaveForceST()
    {
        CachedRange<string>.SaveSinglethreaded<string>(Range, TimeSpan.FromHours(3));
    }

    [Benchmark]
    public void SaveMemoryCacheMT()
    {
        var range = Range;
        var sliceLength = range.Length / Environment.ProcessorCount;
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        Parallel.For(0, Environment.ProcessorCount, i => WriteSlice(i));

        void WriteSlice(int i)
        {
            var ret = string.Empty;

            var start = i * sliceLength;
            var end = (i + 1) * sliceLength;

            foreach (var (key, value) in range.AsSpan()[start..end])
            {
                _memoryCache.Set(key, value, expiration);
            }
        }
    }

    [Benchmark]
    public void SaveMemoryCache()
    {
        var range = Range;
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        foreach (var (key, value) in range)
        {
            _memoryCache.Set(key, value, expiration);
        }
    }

    [Benchmark]
    public void SaveCacheManager()
    {
        var range = Range;

        foreach (var (key, value) in range)
        {
            _cacheManager.Put(key, value);
        }
    }

    [Benchmark]
    public void SaveLazyCache()
    {
        var range = Range;
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        foreach (var (key, value) in range)
        {
           _lazyCache.Add(key, value, expiration);
        }
    }
}
