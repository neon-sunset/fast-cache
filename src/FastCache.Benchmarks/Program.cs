using BenchmarkDotNet.Running;
using FastCache.Benchmarks;

BenchmarkRunner.Run<RemoveExpiredEntriesBenchmark>();