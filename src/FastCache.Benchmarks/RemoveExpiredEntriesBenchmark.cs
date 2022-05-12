using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Services;

namespace FastCache.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3, printSource: true, exportHtml: true)]
[SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess)]
public class RemoveExpiredEntriesBenchmark
{
    // readonly int tickCount = Environment.TickCount64;

    [GlobalSetup]
    public void Initialize()
    {
        const int limit = 32768;

        var evictionJob = Cached<string>.s_evictionJob;

        evictionJob.QuickListEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        evictionJob.FullEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);

        // var ticksMax = TimeSpan.FromSeconds(90).Ticks;

        for (int i = 0; i < limit; i++)
        {
            // var expiration = TimeSpan.FromTicks(Random.Shared.NextInt64(1, ticksMax));
            _ = $"string value of {i}".Cache(i, TimeSpan.FromMilliseconds(0.1));
        }

        for (int i = 0; i < limit; i++)
        {
            // var expiration = TimeSpan.FromTicks(Random.Shared.NextInt64(1, ticksMax));
            _ = $"string value of {i}".Cache(i, TimeSpan.FromHours(1));
        }
    }

    [Benchmark]
    public void Run() => CacheManager.EvictFromQuickList<string>(Environment.TickCount64);
}
