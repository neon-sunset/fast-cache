using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace FastCache.Benchmarks;

[SimpleJob(RuntimeMoniker.HostProcess)]
[MemoryDiagnoser]
public class CachedLastBenchmarks
{
    [Benchmark]
    public string TryGetNone()
    {
        if (Cached<string>.TryGetLast(out var value))
        {
            return value;
        }

        return "default".CacheIndefinitely();
    }

    [Benchmark]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGetLast("one", out var cached))
        {
            return cached.Value;
        }

        return cached.SaveIndefinitely("single");
    }

    [Benchmark]
    public string TryGetEight()
    {
        if (Cached<string>.TryGetLast("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.SaveIndefinitely("eight");
    }

    [Benchmark]
    public string GetAndSaveSingle()
    {
        if (!Cached<string>.TryGetLast("one", out var cached))
        {
            return cached.Value;
        }

        return cached.SaveIndefinitely("single");
    }

    [Benchmark]
    public string GetAndSaveEight()
    {
        if (!Cached<string>.TryGetLast("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.SaveIndefinitely("eight");
    }

    [Benchmark]
    public string GetOrCompute() => Cached.LastOrCompute("new computed value", v => Delegate(v));

    private static string Delegate(string input) => $"computed: {input}";
}
