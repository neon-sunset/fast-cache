using BenchmarkDotNet.Running;
using FastCache;
using FastCache.Benchmarks;
using FastCache.Jobs;

const int parallelism = 16;
const int limit = 10000 / parallelism;

Parallel.For(0, parallelism, static num => Seed(num, limit));
Thread.Sleep(2);

RemoveExpiredEntriesJob.Run<string>();

static void Seed(int num, int limit)
{
    for (int i = 0; i < limit; i++)
    {
        $"string value of {i} at {num}".Cache(i, num, TimeSpan.FromMilliseconds(1));
    }
}

// BenchmarkRunner.Run<RemoveExpiredEntriesBenchmark>();