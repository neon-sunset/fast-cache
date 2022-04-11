using BenchmarkDotNet.Running;
using FastCache;
using FastCache.Benchmarks;
using FastCache.Jobs;

BenchmarkRunner.Run<RemoveExpiredEntriesBenchmark>();