using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FastCache.Services;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess, runStrategy: RunStrategy.Throughput, targetCount: 500)]
public class RemoveExpiredEntriesBenchmark
{
    [IterationSetup]
    public void Initialize()
    {
        const int parallelism = 1;
        const int limit = 30000 / parallelism;
        Parallel.For(0, parallelism, static num => Seed(num, limit));

        static void Seed(int num, int limit)
        {
            for (int i = 0; i < limit; i++)
            {
                var exp = Random.Shared.Next(0, 10) > 4 ? TimeSpan.FromHours(1) : TimeSpan.FromMilliseconds(1);
                $"string value of {i} and {num}".Cache(i, exp);
            }
        }

        Thread.Sleep(2);
    }

    [Benchmark]
    public async ValueTask Run() => await CacheManager.PerformFullEviction<string>();
}