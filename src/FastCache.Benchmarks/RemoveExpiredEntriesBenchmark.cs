using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FastCache.Jobs;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess, runStrategy: RunStrategy.Throughput, targetCount: 500)]
public class RemoveExpiredEntriesBenchmark
{
    private static readonly TimeSpan Expiration = TimeSpan.FromMilliseconds(1);

    [IterationSetup]
    public void Initialize()
    {
        const int parallelism = 1;
        const int limit = 10000 / parallelism;
        Parallel.For(0, parallelism, static num => Seed(num, limit));

        static void Seed(int num, int limit)
        {
            for (int i = 0; i < limit; i++)
            {
                $"string value of {i} at {num}".Cache(i, num, Expiration);
            }
        }

        Thread.Sleep(2);
    }

    [Benchmark]
    public void Run() => CacheItemsEvictionJob.RunInternal<string>();
}