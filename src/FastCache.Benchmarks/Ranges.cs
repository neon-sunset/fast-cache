using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CacheManager.Core;
using FastCache.Collections;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

// WARNING: Takes 4-8 GB of RAM to run 10-20M
//[SimpleJob(RuntimeMoniker.HostProcess, warmupCount: 5, targetCount: 10)] - .NET 7 is too unstable for now
[SimpleJob(RuntimeMoniker.Net60, warmupCount: 5, targetCount: 10)]
[MemoryDiagnoser]
public class Ranges
{
    private const string ItemValue = "1337";

    private static readonly (string, string)[] Range = Enumerable.Range(0, 20_000_000).Select(i => (i.ToString(), ItemValue)).ToArray();

    [Params(1000, 100_000, 1_000_000, 10_000_000, 20_000_000)]
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
    }

    [Benchmark(Baseline = true)]
    public void Save()
    {
        CachedRange.Save(GetRange(), TimeSpan.FromHours(3));
    }

    [Benchmark]
    public void SaveForceST()
    {
        CachedRange.SaveSinglethreaded(GetRange(), TimeSpan.FromHours(3));
    }

    [Benchmark]
    public void SaveMemoryCache()
    {
        var range = GetRange();
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        foreach (var (key, value) in range.Span)
        {
            _memoryCache.Set(key, value, expiration);
        }
    }

    [Benchmark]
    public void SaveCacheManager()
    {
        var range = GetRange();

        foreach (var (key, value) in range.Span)
        {
            _cacheManager.Put(key, value);
        }
    }

    [Benchmark]
    public void SaveLazyCache()
    {
        var range = GetRange();
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        foreach (var (key, value) in range.Span)
        {
           _lazyCache.Add(key, value, expiration);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public ReadOnlyMemory<(string, string)> GetRange() => Length switch
    {
        <= 20_000_000 => Range[..Length],
        _ => throw new ArgumentOutOfRangeException(nameof(Length))
    };
}
