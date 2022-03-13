using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace FastCache.Benchmarks;

// [SimpleJob(RuntimeMoniker.HostProcess)]
// [SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[MemoryDiagnoser]
public class CacheBenchmarks
{
    private static readonly TimeSpan Expiration = TimeSpan.FromMinutes(10);

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

    [Benchmark]
    public string? LastSingle() => Cached<string>.Last("one");

    [Benchmark]
    public string? LastEight() => Cached<string>.Last("one", "two", "three", "four", "five", "six", "seven", "eight");

    [Benchmark]
    public string? LastNine() => Cached<string>.Last("one", "two", "three", "four", "five", "six", "seven", "eight", "nine");

    [Benchmark]
    public string TryGetNone()
    {
        if (Cached<string>.TryGetLast(out var value))
        {
            return value;
        }

        return "default".Cache();
    }

    [Benchmark]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGetLast("one", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single");
    }

    [Benchmark]
    public string TryGetEight()
    {
        if (Cached<string>.TryGetLast("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("eight");
    }

    [Benchmark]
    public string GetAndSaveSingle()
    {
        if (!Cached<string>.TryGetLast("one", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single");
    }

    [Benchmark]
    public string GetAndSaveEight()
    {
        if (!Cached<string>.TryGetLast("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("eight");
    }

    [Benchmark]
    public string GetAndSaveSingleWithExpiration()
    {
        if (!Cached<string>.TryGet("one", Expiration, out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single");
    }
}