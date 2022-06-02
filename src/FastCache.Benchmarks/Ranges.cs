using BenchmarkDotNet.Attributes;
using FastCache.Collections;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 5, printSource: true, exportCombinedDisassemblyReport: true)]
public class Ranges
{
    private const string ItemValue = "item value";

    public List<(int, string)[]> Inputs = new()
    {
        Enumerable.Range(0, 1).Select(i => (i, ItemValue)).ToArray(),
        Enumerable.Range(0, 100).Select(i => (i, ItemValue)).ToArray(),
        Enumerable.Range(0, 1000).Select(i => (i, ItemValue)).ToArray(),
        Enumerable.Range(0, 100_000).Select(i => (i, ItemValue)).ToArray(),
        Enumerable.Range(0, 1_000_000).Select(i => (i, ItemValue)).ToArray()
    };

    [GlobalSetup]
    public void GlobalSetup()
    {
        Services.CacheManager.SuspendEviction<int, string>();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        Services.CacheManager.QueueFullClear<int, string>();
        Thread.Sleep(500);
    }

    [Benchmark(OperationsPerInvoke = 1)]
    public void Save1() => CachedRange<string>.Save(Inputs[0].AsSpan(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 100)]
    public void Save100() => CachedRange<string>.Save(Inputs[1].AsSpan(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 1000)]
    public void Save1000() => CachedRange<string>.Save(Inputs[2].AsSpan(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 1000)]
    public void Save1000MT() => CachedRange<string>.SaveMultithreaded(Inputs[2].AsMemory(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 100_000)]
    public void Save100_000() => CachedRange<string>.Save(Inputs[3].AsSpan(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 100_000)]
    public void Save100_000MT() => CachedRange<string>.SaveMultithreaded(Inputs[3].AsMemory(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 1_000_000)]
    public void Save1_000_000() => CachedRange<string>.Save(Inputs[4].AsSpan(), TimeSpan.FromHours(3));

    [Benchmark(OperationsPerInvoke = 1_000_000)]
    public void Save1_000_000MT() => CachedRange<string>.SaveMultithreaded(Inputs[4].AsMemory(), TimeSpan.FromHours(3));
}
