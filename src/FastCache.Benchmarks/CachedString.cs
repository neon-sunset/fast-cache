using BenchmarkDotNet.Attributes;
using FastCache.Extensions;
using FastCache.Services;

namespace FastCache.Benchmarks;

// [SimpleJob(RuntimeMoniker.HostProcess)]
// [DisassemblyDiagnoser(maxDepth: 3, printSource: true, exportHtml: true)]
[MemoryDiagnoser]
public class CachedString
{
    private static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);

    [GlobalSetup]
    public void Initialize()
    {
        CacheManager.SuspendEviction<string>();

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
    public void SaveSingle()
    {
        _ = "single".Cache("one", OneHour);
    }

    [Benchmark]
    public void SaveEight()
    {
        _ = "eight".Cache("one", "two", "three", "four", "five", "six", "seven", "eight", OneHour);
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

    [Benchmark]
    public string GetOrCompute() => Cached.GetOrCompute("new computed value", Delegate, OneHour);

    private static string Delegate(string input) => $"computed: {input}";
}
