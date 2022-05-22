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
        // CacheManager.SuspendEviction<string>();

        for (uint i = 0; i < 1000; i++)
        {
            $"seeded string number {i}".Cache(i, OneHour);
        }

        "single".Cache("one", OneHour);
        "eight".Cache("one", "two", "three", "four", "five", "six", "seven", "eight", OneHour);
    }

    [Benchmark(Baseline = true)]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGet("one", out var cached))
        {
            return cached.Value;
        }

        return cached.Save("single", OneHour);
    }

    [Benchmark]
    public string TryGetRandomSingle()
    {
        var key = (uint)Random.Shared.Next(0, 1000);
        if (Cached<string>.TryGet(key, out var cached))
        {
            return cached.Value;
        }

        return cached.Save("replaced single random", OneHour);
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
    public string GetOrCompute() => Cached.GetOrCompute("new computed value", param => Delegate(param), OneHour);

    [Benchmark]
    public async ValueTask<string> GetOrComputeValueTask() => await Cached.GetOrCompute("new computed value", param => DelegateValueTask(param), OneHour);

    [Benchmark]
    public async Task<string> GetOrComputeTask() => await Cached.GetOrCompute("new computed value", param => DelegateTask(param), OneHour);

    private static string Delegate(string input) => $"computed: {input}";

    private static ValueTask<string> DelegateValueTask(string input) => ValueTask.FromResult($"computed: {input}");

    private static Task<string> DelegateTask(string input) => Task.FromResult($"computed: {input}");
}
