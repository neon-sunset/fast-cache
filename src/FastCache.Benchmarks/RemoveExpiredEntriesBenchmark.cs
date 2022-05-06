using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Services;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess)]
public class RemoveExpiredEntriesBenchmark
{
    readonly int tickCount = Environment.TickCount;

    [IterationSetup]
    public void Initialize()
    {
        const int limit = 32000;

        // var ticksMax = TimeSpan.FromSeconds(30).Ticks;

        for (int i = 0; i < limit; i++)
        {
            // var expiration = TimeSpan.FromTicks(Random.Shared.NextInt64(0, ticksMax));
            $"string value of {i}".Cache(i, TimeSpan.Zero);
        }
    }

    [Benchmark]
    public void Run() => CacheManager.EvictFromQuickList<string>(tickCount);
}
