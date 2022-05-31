using BenchmarkDotNet.Attributes;
using FastCache.Extensions;

namespace FastCache.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class Defaults
{
    private static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);

    [GlobalSetup]
    public void Initialize()
    {
        Services.CacheManager.SuspendEviction<string, string>();
        Services.CacheManager.SuspendEviction<(string, string), string>();
        Services.CacheManager.SuspendEviction<(string, string, string), string>();
        Services.CacheManager.SuspendEviction<(string, string, string, string), string>();
        Services.CacheManager.SuspendEviction<(string, string, string, string, string, string, string), string>();

        Services.CacheManager.SuspendEviction<int, string>();
        Services.CacheManager.SuspendEviction<(int, int), string>();
        Services.CacheManager.SuspendEviction<(int, int, int), string>();
        Services.CacheManager.SuspendEviction<(int, int, int, int), string>();
        Services.CacheManager.SuspendEviction<(int, int, int, int, int, int, int), string>();

        Services.CacheManager.SuspendEviction<(string, int), string>();
        Services.CacheManager.SuspendEviction<(string, int, string), string>();
        Services.CacheManager.SuspendEviction<(string, int, string, int), string>();
        Services.CacheManager.SuspendEviction<(string, int, string, int, string, int, string), string>();

        "single".Cache("one", OneHour);
        "two".Cache("one", "two", OneHour);
        "three".Cache("one", "two", "three", OneHour);
        "four".Cache("one", "two", "three", "four", OneHour);
        "seven".Cache("one", "two", "three", "four", "five", "six", "seven", OneHour);

        "single".Cache(1, OneHour);
        "two".Cache(1, 2, OneHour);
        "three".Cache(1, 2, 3, OneHour);
        "four".Cache(1, 2, 3, 4, OneHour);
        "seven".Cache(1, 2, 3, 4, 5, 6, 7, OneHour);

        "two".Cache("one", 2, OneHour);
        "three".Cache("one", 2, "three", OneHour);
        "four".Cache("one", 2, "three", 4, OneHour);
        "seven".Cache("one", 2, "three", 4, "five", 6, "seven", OneHour);
    }

    [Benchmark(Baseline = true)]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGet("one", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwo()
    {
        if (Cached<string>.TryGet("one", "two", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThree()
    {
        if (Cached<string>.TryGet("one", "two", "three", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFour()
    {
        if (Cached<string>.TryGet("one", "two", "three", "four", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetSeven()
    {
        if (Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetSingleInt()
    {
        if (Cached<string>.TryGet(1, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwoInt()
    {
        if (Cached<string>.TryGet(1, 2, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThreeInt()
    {
        if (Cached<string>.TryGet(1, 2, 3, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFourInt()
    {
        if (Cached<string>.TryGet(1, 2, 3, 4, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetSevenInt()
    {
        if (Cached<string>.TryGet(1, 2, 3, 4, 5, 6, 7, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwoMixed()
    {
        if (Cached<string>.TryGet("one", 2, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThreeMixed()
    {
        if (Cached<string>.TryGet("one", 2, "three", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFourMixed()
    {
        if (Cached<string>.TryGet("one", 2, "three", 4, out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetSevenMixed()
    {
        if (Cached<string>.TryGet("one", 2, "three", 4, "five", 6, "seven", out var cached))
        {
            return cached.Value;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string GetAndSaveSingle()
    {
        Cached<string>.TryGet("one", out var cached);
        return cached.Save("single", OneHour);
    }

    [Benchmark]
    public string GetAndSaveTwo()
    {
        Cached<string>.TryGet("one", "two", out var cached);
        return cached.Save("two", OneHour);
    }

    [Benchmark]
    public string GetAndSaveThree()
    {
        Cached<string>.TryGet("one", "two", "three", out var cached);
        return cached.Save("three", OneHour);
    }

    [Benchmark]
    public string GetAndSaveFour()
    {
        Cached<string>.TryGet("one", "two", "three", "four", out var cached);
        return cached.Save("four", OneHour);
    }

    [Benchmark]
    public string GetAndSaveSeven()
    {
        Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", out var cached);
        return cached.Save("seven", OneHour);
    }

    [Benchmark]
    public string GetAndSaveSingleInt()
    {
        Cached<string>.TryGet(1, out var cached);
        return cached.Save("single", OneHour);
    }

    [Benchmark]
    public string GetAndSaveTwoInt()
    {
        Cached<string>.TryGet(1, 2, out var cached);
        return cached.Save("two", OneHour);
    }

    [Benchmark]
    public string GetAndSaveThreeInt()
    {
        Cached<string>.TryGet(1, 2, 3, out var cached);
        return cached.Save("three", OneHour);
    }

    [Benchmark]
    public string GetAndSaveFourInt()
    {
        Cached<string>.TryGet(1, 2, 3, 4, out var cached);
        return cached.Save("four", OneHour);
    }

    [Benchmark]
    public string GetAndSaveSevenInt()
    {
        Cached<string>.TryGet(1, 2, 3, 4, 5, 6, 7, out var cached);
        return cached.Save("seven", OneHour);
    }

    [Benchmark]
    public string GetAndSaveTwoMixed()
    {
        Cached<string>.TryGet("one", 2, out var cached);
        return cached.Save("two", OneHour);
    }

    [Benchmark]
    public string GetAndSaveThreeMixed()
    {
        Cached<string>.TryGet("one", 2, "three", out var cached);
        return cached.Save("three", OneHour);
    }

    [Benchmark]
    public string GetAndSaveFourMixed()
    {
        Cached<string>.TryGet("one", 2, "three", 4, out var cached);
        return cached.Save("four", OneHour);
    }

    [Benchmark]
    public string GetAndSaveSevenMixed()
    {
        Cached<string>.TryGet("one", 2, "three", 4, "five", 6, "seven", out var cached);
        return cached.Save("seven", OneHour);
    }

    [Benchmark]
    public void CacheSingle()
    {
        "single".Cache("one", OneHour);
    }

    [Benchmark]
    public void CacheTwo()
    {
        "two".Cache("one", "two", OneHour);
    }

    [Benchmark]
    public void CacheThree()
    {
        "three".Cache("one", "two", "three", OneHour);
    }

    [Benchmark]
    public void CacheFour()
    {
        "four".Cache("one", "two", "three", "four", OneHour);
    }

    [Benchmark]
    public void CacheSeven()
    {
        "seven".Cache("one", "two", "three", "four", "five", "six", "seven", OneHour);
    }

    [Benchmark]
    public void CacheSingleInt()
    {
        "single".Cache(1, OneHour);
    }

    [Benchmark]
    public void CacheTwoInt()
    {
        "two".Cache(1, 2, OneHour);
    }

    [Benchmark]
    public void CacheThreeInt()
    {
        "three".Cache(1, 2, 3, OneHour);
    }

    [Benchmark]
    public void CacheFourInt()
    {
        "four".Cache(1, 2, 3, 4, OneHour);
    }

    [Benchmark]
    public void CacheSevenInt()
    {
        "seven".Cache(1, 2, 3, 4, 5, 6, 7, OneHour);
    }

    [Benchmark]
    public void CacheTwoMixed()
    {
        "two".Cache("one", 2, OneHour);
    }

    [Benchmark]
    public void CacheThreeMixed()
    {
        "three".Cache("one", 2, "three", OneHour);
    }

    [Benchmark]
    public void CacheFourMixed()
    {
        "four".Cache("one", 2, "three", 4, OneHour);
    }

    [Benchmark]
    public void CacheSevenMixed()
    {
        "seven".Cache("one", 2, "three", 4, "five", 6, "seven", OneHour);
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