using BenchmarkDotNet.Running;
using FastCache;
using FastCache.Benchmarks;

BenchmarkRunner.Run<RemoveExpiredEntriesBenchmark>();