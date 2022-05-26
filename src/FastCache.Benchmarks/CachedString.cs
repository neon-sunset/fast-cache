using BenchmarkDotNet.Attributes;
using FastCache.Extensions;

namespace FastCache.Benchmarks
{
    // [SimpleJob(RuntimeMoniker.HostProcess)]
    // [DisassemblyDiagnoser(maxDepth: 3, printSource: true, exportHtml: true)]
    [MemoryDiagnoser]
    public class CachedString
    {
        private static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);
        private static readonly Random Random = new();

        [GlobalSetup]
        public void Initialize()
        {
            // CacheManager.SuspendEviction<string>();
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
        public string TryGetTwo()
        {
            if (Cached<string>.TryGet("one", "two", out var cached))
            {
                return cached.Value;
            }

            return cached.Save("two", OneHour);
        }

        [Benchmark]
        public string TryGetFour()
        {
            if (Cached<string>.TryGet("one", "two", "three", "four", out var cached))
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
        public void SaveTwo()
        {
            _ = "two".Cache("one", "two", OneHour);
        }

        [Benchmark]
        public void SaveFour()
        {
            _ = "eight".Cache("one", "two", "three", "four", OneHour);
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

        // [Benchmark]
        public string GetAndSaveSeven()
        {
            if (!Cached<string>.TryGet("one", "two", "three", "four", "five", "six", "seven", out var cached))
            {
                return cached.Value;
            }

            return cached.Save("seven", OneHour);
        }

        [Benchmark]
        public string GetOrCompute() => Cached.GetOrCompute("new computed value", param => Delegate(param), OneHour);

        // [Benchmark]
        // public async ValueTask<string> GetOrComputeValueTask() => await Cached.GetOrCompute("new computed value", param => DelegateValueTask(param), OneHour);

        [Benchmark]
        public async Task<string> GetOrComputeTask() => await Cached.GetOrCompute("new computed value", param => DelegateTask(param), OneHour);

        private static string Delegate(string input) => $"computed: {input}";

        // private static ValueTask<string> DelegateValueTask(string input) => ValueTask.FromResult($"computed: {input}");

        private static Task<string> DelegateTask(string input) => Task.FromResult($"computed: {input}");
    }
}