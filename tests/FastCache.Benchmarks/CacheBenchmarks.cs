using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace FastCache.Benchmarks;

[SimpleJob(RuntimeMoniker.HostProcess)]
[SimpleJob(RuntimeMoniker.Net60, baseline: true)]
[MemoryDiagnoser]
public class CacheBenchmarks
{
    private static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);

    [GlobalSetup]
    public void Initialize()
    {
        _ = "default".Cache(OneHour);
        _ = "single".Cache("one", OneHour);
        _ = "eight".Cache("one", "two", "three", "four", "five", "six", "seven", "eight", OneHour);
        _ = "nine".Cache(OneHour, "one", "two", "three", "four", "five", "six", "seven", "eight", "nine");
    }

    [Benchmark]
    public string TryGetNone()
    {
        if (Cached<string>.TryGet(out var value))
        {
            return value;
        }

        return "default".Cache(OneHour);
    }

    [Benchmark]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGet("one", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single", OneHour);
    }

    [Benchmark]
    public string TryGetEight()
    {
        if (Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("eight", OneHour);
    }

    [Benchmark]
    public string GetAndSaveSingle()
    {
        if (!Cached<string>.TryGet("one", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single", OneHour);
    }

    [Benchmark]
    public string GetAndSaveEight()
    {
        if (!Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", "eight", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("eight", OneHour);
    }
}
