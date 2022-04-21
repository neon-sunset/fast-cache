using BenchmarkDotNet.Running;
using FastCache;
using FastCache.Benchmarks;
using FastCache.Services;

BenchmarkRunner.Run<CachedLastBenchmarks>();