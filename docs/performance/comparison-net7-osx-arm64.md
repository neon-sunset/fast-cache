``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.4 (21F79) [Darwin 21.5.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=7.0.100-preview.6.22276.5
  [Host]     : .NET 7.0.0 (7.0.22.27203), Arm64 RyuJIT
  DefaultJob : .NET 7.0.0 (7.0.22.27203), Arm64 RyuJIT


```
|             Method |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|------------------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|       TryGetCached |  16.14 ns | 0.012 ns | 0.011 ns |  1.00 |    0.00 |      - |         - |
|  TryGetMemoryCache |  33.84 ns | 0.017 ns | 0.015 ns |  2.10 |    0.00 |      - |         - |
| TryGetCacheManager |  44.34 ns | 0.016 ns | 0.014 ns |  2.75 |    0.00 |      - |         - |
|    TryGetLazyCache |  56.67 ns | 0.686 ns | 0.642 ns |  3.51 |    0.04 |      - |         - |
|       UpdateCached |  59.82 ns | 0.166 ns | 0.147 ns |  3.71 |    0.01 | 0.0063 |      40 B |
|  UpdateMemoryCache | 190.16 ns | 2.144 ns | 2.006 ns | 11.79 |    0.12 | 0.0355 |     224 B |
| UpdateCacheManager | 337.19 ns | 0.329 ns | 0.292 ns | 20.89 |    0.02 | 0.0572 |     360 B |
|    UpdateLazyCache | 249.35 ns | 0.420 ns | 0.350 ns | 15.45 |    0.03 | 0.0763 |     480 B |
