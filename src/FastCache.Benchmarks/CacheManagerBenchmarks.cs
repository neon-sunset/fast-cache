using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using FastCache.Extensions;

namespace FastCache.Benchmarks
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(maxDepth: 3, printSource: true, exportHtml: true)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess)]
    public class CacheManagerBenchmarks
    {
        // readonly int tickCount = Environment.TickCount64;

        [GlobalSetup]
        public void Initialize()
        {
            const int limit = 32768;

            // CacheManager.SuspendEviction<string>();

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
        public void Run() => CacheStaticHolder<int, string>.s_quickList.Evict();
    }
}