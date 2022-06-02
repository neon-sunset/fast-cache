using BenchmarkDotNet.Attributes;
using FastCache.Collections;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
public class Ranges
{
    private const string ItemValue = "1337";

    public IEnumerable<(int, string)[]> Inputs()
    {
        yield return Enumerable.Range(0, 1000).Select(i => (i, ItemValue)).ToArray();
        yield return Enumerable.Range(0, 10_000).Select(i => (i, ItemValue)).ToArray();
        yield return Enumerable.Range(0, 100_000).Select(i => (i, ItemValue)).ToArray();
        yield return Enumerable.Range(0, 262_144).Select(i => (i, ItemValue)).ToArray();
        yield return Enumerable.Range(0, 1_000_000).Select(i => (i, ItemValue)).ToArray();
        yield return Enumerable.Range(0, 10_000_000).Select(i => (i, ItemValue)).ToArray();
    }

    [ParamsSource(nameof(Inputs))]
    public (string, string)[] Value = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Services.CacheManager.SuspendEviction<int, string>();
    }

    [Benchmark(Baseline = true)]
    public void Save() => CachedRange.Save(Value.AsSpan(), TimeSpan.FromHours(3));

    [Benchmark]
    public void SaveMultithreaded() => CachedRange.SaveMultithreaded(Value.AsMemory(), TimeSpan.FromHours(3));

    [Benchmark]
    public void SaveEnumerable() => CachedRange.Save(Value, TimeSpan.FromHours(3));
}
