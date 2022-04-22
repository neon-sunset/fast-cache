using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FastCache.Services;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess, invocationCount: 8192)]
public class RemoveExpiredEntriesBenchmark
{
    [GlobalSetup]
    public void Initialize()
    {
        const int parallelism = 1;
        const int limit = 32000 / parallelism;
        Parallel.For(0, parallelism, static num => Seed(num, limit));

        static void Seed(int num, int limit)
        {
            var ticksMax = TimeSpan.FromSeconds(30).Ticks;

            for (int i = 0; i < limit; i++)
            {
                var expTicks = Random.Shared.NextInt64(0, ticksMax);
                $"string value of {i} and {num}".Cache(i, TimeSpan.FromTicks(expTicks));
            }
        }
    }

    [Benchmark]
    public void Run() => CacheManager.EvictFromQuickList<string>(DateTime.UtcNow.Ticks);
}
