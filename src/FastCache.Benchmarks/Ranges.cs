using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using FastCache.Collections;
using Microsoft.Extensions.Caching.Memory;

namespace FastCache.Benchmarks;

// [RPlotExporter]
// [CsvMeasurementsExporter(CsvSeparator.Comma)]
[MemoryDiagnoser]
public class Ranges
{
    private const string ItemValue = "1337";

    public IEnumerable<(string, string)[]> Inputs()
    {
        yield return Enumerable.Range(0, 100).Select(i => (i.ToString(), ItemValue)).ToArray();
        yield return Enumerable.Range(0, 1000).Select(i => (i.ToString(), ItemValue)).ToArray();
        yield return Enumerable.Range(0, 10_000).Select(i => (i.ToString(), ItemValue)).ToArray();
        yield return Enumerable.Range(0, 100_000).Select(i => (i.ToString(), ItemValue)).ToArray();
        yield return Enumerable.Range(0, 1_000_000).Select(i => (i.ToString(), ItemValue)).ToArray();
        yield return Enumerable.Range(0, 10_000_000).Select(i => (i.ToString(), ItemValue)).ToArray();
    }

    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    [GlobalSetup]
    public void GlobalSetup()
    {
        Services.CacheManager.SuspendEviction<string, string>();
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Inputs))]
    public void Save((string, string)[] range)
    {
        CachedRange.Save(range, TimeSpan.FromHours(3));
    }

    // [Benchmark]
    // [ArgumentsSource(nameof(Inputs))]
    // public void SaveEnumerable<K, V>(IEnumerable<(K, V)> range)
    // {
    //     CachedRange.Save(range, TimeSpan.FromHours(3));
    // }

    [Benchmark]
    [ArgumentsSource(nameof(Inputs))]
    public void SaveMemoryCache((string, string)[] range)
    {
        var expiration = DateTimeOffset.UtcNow + TimeSpan.FromHours(3);

        foreach (var (key, value) in range)
        {
            _memoryCache.Set(key, value, expiration);
        }
    }
}
