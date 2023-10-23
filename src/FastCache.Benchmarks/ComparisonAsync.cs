using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;

namespace FastCache.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 5, exportCombinedDisassemblyReport: true)]
public class ComparisonAsync
{
    [Params("A", "abcd", "long ass string with букви кирилицею AND UPPERCASE")]
    public string ItemKey = default!;
    public string ItemValue = "item value";

    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

    private readonly IAppCache _lazyCache = new CachingService();
    private readonly FusionCache _fusionCache = new (new FusionCacheOptions());

    private readonly TimeSpan _expire = TimeSpan.FromMinutes(60);

    [GlobalSetup]
    public void Initialize()
    {
        Services.CacheManager.SuspendEviction<string, string>();
    }

    [Benchmark(Baseline = true)]
    public async Task<string> GetOrComputeCached()
    {
        return await Cached.GetOrCompute(ItemKey, GetString, _expire);
    }

    [Benchmark]
    public async Task<string?> GetOrComputeMemoryCache()
    {
        return await _memoryCache.GetOrCreateAsync(ItemKey, async factory =>
        {
            factory.SetAbsoluteExpiration(_expire);
            return await GetString(ItemKey);
        }) ?? Unreachable<string>();
    }

    [Benchmark]
    public async Task<string> GetOrComputeLazyCache()
    {
        return await _lazyCache.GetOrAddAsync<string>(ItemKey, async factory =>
        {
            factory.SetAbsoluteExpiration(_expire);
            return await GetString(ItemKey);
        });
    }

    [Benchmark]
    public async Task<string> GetOrComputeFusionCache()
    {
        return await _fusionCache.GetOrSetAsync<string>(ItemKey, async (context, _) =>
        {
            context.Options.SetDuration(_expire);
            return await GetString(ItemKey);
        }) ?? Unreachable<string>();
    }

    private async Task<string> GetString(string key)
    {
        await Task.Delay(10);
        return await Task.FromResult(ItemValue);
    }
}