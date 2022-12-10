using FastCache.Extensions;

namespace FastCache.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
// [DisassemblyDiagnoser(maxDepth: 10, printSource: true, exportCombinedDisassemblyReport: true)]
public class Defaults
{
    [GlobalSetup]
    public void Initialize()
    {
        "single".Cache("one", TimeSpan.FromMinutes(60));
        "two".Cache("one", "two", TimeSpan.FromMinutes(60));
        "three".Cache("one", "two", "three", TimeSpan.FromMinutes(60));
        "four".Cache("one", "two", "three", "four", TimeSpan.FromMinutes(60));

        "single".Cache(1, TimeSpan.FromMinutes(60));
        "two".Cache(1, 2, TimeSpan.FromMinutes(60));
        "three".Cache(1, 2, 3, TimeSpan.FromMinutes(60));
        "four".Cache(1, 2, 3, 4, TimeSpan.FromMinutes(60));

        "two".Cache("one", 2, TimeSpan.FromMinutes(60));
        "three".Cache("one", 2, "three", TimeSpan.FromMinutes(60));
        "four".Cache("one", 2, "three", 4, TimeSpan.FromMinutes(60));
    }

    [Benchmark(Baseline = true)]
    public string TryGetSingle()
    {
        if (Cached<string>.TryGet("one", out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwo()
    {
        if (Cached<string>.TryGet("one", "two", out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThree()
    {
        if (Cached<string>.TryGet("one", "two", "three", out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFour()
    {
        if (Cached<string>.TryGet("one", "two", "three", "four", out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetSingleInt()
    {
        if (Cached<string>.TryGet(1, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwoInt()
    {
        if (Cached<string>.TryGet(1, 2, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThreeInt()
    {
        if (Cached<string>.TryGet(1, 2, 3, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFourInt()
    {
        if (Cached<string>.TryGet(1, 2, 3, 4, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetTwoMixed()
    {
        if (Cached<string>.TryGet("one", 2, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetThreeMixed()
    {
        if (Cached<string>.TryGet("one", 2, "three", out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string TryGetFourMixed()
    {
        if (Cached<string>.TryGet("one", 2, "three", 4, out var cached))
        {
            return cached;
        }

        return Unreachable<string>();
    }

    [Benchmark]
    public string GetAndSaveSingle()
    {
        Cached<string>.TryGet("one", out var cached);
        return cached.Save("single", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveTwo()
    {
        Cached<string>.TryGet("one", "two", out var cached);
        return cached.Save("two", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveThree()
    {
        Cached<string>.TryGet("one", "two", "three", out var cached);
        return cached.Save("three", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveFour()
    {
        Cached<string>.TryGet("one", "two", "three", "four", out var cached);
        return cached.Save("four", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveSingleInt()
    {
        Cached<string>.TryGet(1, out var cached);
        return cached.Save("single", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveTwoInt()
    {
        Cached<string>.TryGet(1, 2, out var cached);
        return cached.Save("two", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveThreeInt()
    {
        Cached<string>.TryGet(1, 2, 3, out var cached);
        return cached.Save("three", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveFourInt()
    {
        Cached<string>.TryGet(1, 2, 3, 4, out var cached);
        return cached.Save("four", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveTwoMixed()
    {
        Cached<string>.TryGet("one", 2, out var cached);
        return cached.Save("two", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveThreeMixed()
    {
        Cached<string>.TryGet("one", 2, "three", out var cached);
        return cached.Save("three", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetAndSaveFourMixed()
    {
        Cached<string>.TryGet("one", 2, "three", 4, out var cached);
        return cached.Save("four", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheSingle()
    {
        "single".Cache("one", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheTwo()
    {
        "two".Cache("one", "two", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheThree()
    {
        "three".Cache("one", "two", "three", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheFour()
    {
        "four".Cache("one", "two", "three", "four", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheSingleInt()
    {
        "single".Cache(1, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheTwoInt()
    {
        "two".Cache(1, 2, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheThreeInt()
    {
        "three".Cache(1, 2, 3, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheFourInt()
    {
        "four".Cache(1, 2, 3, 4, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheTwoMixed()
    {
        "two".Cache("one", 2, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheThreeMixed()
    {
        "three".Cache("one", 2, "three", TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public void CacheFourMixed()
    {
        "four".Cache("one", 2, "three", 4, TimeSpan.FromMinutes(60));
    }

    [Benchmark]
    public string GetOrCompute() => Cached.GetOrCompute("new computed value", param => Delegate(param), TimeSpan.FromMinutes(60));

    [Benchmark]
    public async ValueTask<string> GetOrComputeValueTask() => await Cached.GetOrCompute("new computed value", param => DelegateValueTask(param), TimeSpan.FromMinutes(60));

    [Benchmark]
    public async Task<string> GetOrComputeTask() => await Cached.GetOrCompute("new computed value", param => DelegateTask(param), TimeSpan.FromMinutes(60));

    private static string Delegate(string input) => $"computed: {input}";

    private static ValueTask<string> DelegateValueTask(string input) => new($"computed: {input}");

    private static Task<string> DelegateTask(string input) => Task.FromResult($"computed: {input}");
}