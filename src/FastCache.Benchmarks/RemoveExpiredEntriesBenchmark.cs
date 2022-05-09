using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Services;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess, invocationCount: 1, targetCount: 500)]
public class RemoveExpiredEntriesBenchmark
{
    // readonly int tickCount = Environment.TickCount64;

    [GlobalSetup]
    public void Initialize()
    {
        const int limit = 32000;

        var evictionJob = Cached<string>.s_evictionJob;

        evictionJob.QuickListEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        evictionJob.FullEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);

        var ticksMax = TimeSpan.FromSeconds(30).Ticks;

        for (int i = 0; i < limit; i++)
        {
            var expiration = TimeSpan.FromTicks(Random.Shared.NextInt64(1, ticksMax));
            _ = $"string value of {i}".Cache(i, expiration);
        }
    }

    [Benchmark]
    public void Run() => CacheManager.EvictFromQuickList<string>(Environment.TickCount64);
}
