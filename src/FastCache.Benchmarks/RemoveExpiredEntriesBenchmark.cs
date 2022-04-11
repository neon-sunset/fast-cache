using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Jobs;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(invocationCount: 100000, runtimeMoniker: RuntimeMoniker.HostProcess)]
public class RemoveExpiredEntriesBenchmark
{
    private static readonly TimeSpan Expiration = TimeSpan.FromMilliseconds(1);

    [IterationSetup]
    public void Initialize()
    {
        const int parallelism = 1;
        const int limit = 6144 / parallelism;
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