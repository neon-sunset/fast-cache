using BenchmarkDotNet.Attributes;

namespace FastCache.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class CacheBenchmarks
{
    [GlobalSetup]
    public void Initialize()
    {
        _ = "default".Cache();
        _ = "single".Cache("one");
        _ = "eight".Cache("one", "two", "three", "four", "five", "six", "seven", "eight");
        _ = "nine".Cache("one", "two", "three", "four", "five", "six", "seven", "eight", "nine");
    }

    [Benchmark]
    public string? LastNone() => Cached<string>.Last();

    [Benchmark(Baseline = true)]
    public string? LastSingle() => Cached<string>.Last("one");

    [Benchmark]
    public string? LastEight() => Cached<string>.Last("one", "two", "three", "four", "five", "six", "seven", "eight");

    [Benchmark]
    public string? LastNine() => Cached<string>.Last("one", "two", "three", "four", "five", "six", "seven", "eight", "nine");

    [Benchmark]
    public bool TryGetNone() => Cached<string>.TryGet(out _);

    [Benchmark]
    public bool TryGetSingle() => Cached<string>.TryGet("one", out _);

    [Benchmark]
    public bool TryGetEight() => Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", "eight", out _);

    [Benchmark]
    public string GetAndSaveSingle()
    {
        _ = Cached<string>.TryGet("one", out var holder);

        return holder.Save("single");
    }

    [Benchmark]
    public string GetAndSaveEight()
    {
        _ = Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", "eight", out var holder);

        return holder.Save("eight");
    }
}