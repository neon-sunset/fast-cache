using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Jobs;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(invocationCount: 1000, runtimeMoniker: RuntimeMoniker.HostProcess)]
[SimpleJob(invocationCount: 1000, runtimeMoniker: RuntimeMoniker.Net60)]
public class RemoveExpiredEntriesBenchmark
{
    private static readonly TimeSpan Expiration = TimeSpan.FromMilliseconds(1);

    [GlobalSetup]
    public void Initialize()
    {
        const int parallelism = 16;
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
    public void Run() => RemoveExpiredEntriesJob.Run<string>();
}