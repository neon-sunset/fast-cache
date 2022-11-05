using FastCache.Collections;

namespace FastCache.Benchmarks;

[ShortRunJob, MemoryDiagnoser]
// [DisassemblyDiagnoser(maxDepth: 3, exportCombinedDisassemblyReport: true)]
public class RangeWritesOverloads
{
    private (int, string)[] _array = Array.Empty<(int, string)>();

    private List<(int, string)> _list = new();

    private (int[], string[]) _kvpArrays = (Array.Empty<int>(), Array.Empty<string>());

    [Params(1000, 1_000_000)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var seed = (0..Length).AsEnumerable().Select(key => (key, $"{key}")).ToArray();

        _array = seed;
        _list = seed.ToList();
        _kvpArrays = (
            seed.Select(kvp => kvp.key).ToArray(),
            seed.Select(kvp => kvp.Item2).ToArray());
    }

    [Benchmark(Baseline = true)]
    public void SaveArray() => CachedRange<string>.Save(_array, TimeSpan.MaxValue);

    [Benchmark]
    public void SaveList() => CachedRange<string>.Save(_list, TimeSpan.MaxValue);

    [Benchmark]
    public void SaveSplitArr() =>
        CachedRange<string>.Save(_kvpArrays.Item1, _kvpArrays.Item2, TimeSpan.MaxValue);

    [Benchmark]
    public void SaveEnumerable() =>
        CachedRange<string>.Save(_array.Select(kvp => kvp), TimeSpan.MaxValue);
}