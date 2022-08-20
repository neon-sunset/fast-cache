``` ini

BenchmarkDotNet=v0.13.1, OS=macOS 13.0 (22A5321d) [Darwin 22.0.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=7.0.100-rc.2.22419.24
  [Host]     : .NET 7.0.0 (7.0.22.41112), Arm64 RyuJIT
  DefaultJob : .NET 7.0.0 (7.0.22.41112), Arm64 RyuJIT


```
|             Method |              ItemKey |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|------------------- |--------------------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|       **TryGetCached** |                    **A** |  **12.17 ns** | **0.040 ns** | **0.038 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache |                    A |  29.17 ns | 0.090 ns | 0.075 ns |  2.40 |    0.01 |      - |         - |
| TryGetCacheManager |                    A |  39.68 ns | 0.065 ns | 0.054 ns |  3.26 |    0.01 |      - |         - |
|    TryGetLazyCache |                    A |  38.98 ns | 0.134 ns | 0.125 ns |  3.20 |    0.02 |      - |         - |
|       UpdateCached |                    A |  25.45 ns | 0.115 ns | 0.102 ns |  2.09 |    0.01 | 0.0008 |      40 B |
|  UpdateMemoryCache |                    A | 139.26 ns | 0.353 ns | 0.313 ns | 11.45 |    0.04 | 0.0043 |     224 B |
| UpdateCacheManager |                    A | 112.01 ns | 0.278 ns | 0.246 ns |  9.21 |    0.04 | 0.0031 |     160 B |
|    UpdateLazyCache |                    A | 198.14 ns | 0.390 ns | 0.365 ns | 16.28 |    0.06 | 0.0093 |     480 B |
|                    |                      |           |          |          |       |         |        |           |
|       **TryGetCached** |                 **abcd** |  **13.56 ns** | **0.072 ns** | **0.060 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache |                 abcd |  31.48 ns | 0.070 ns | 0.062 ns |  2.32 |    0.01 |      - |         - |
| TryGetCacheManager |                 abcd |  40.46 ns | 0.055 ns | 0.046 ns |  2.98 |    0.01 |      - |         - |
|    TryGetLazyCache |                 abcd |  41.97 ns | 0.040 ns | 0.038 ns |  3.10 |    0.01 |      - |         - |
|       UpdateCached |                 abcd |  26.53 ns | 0.075 ns | 0.063 ns |  1.96 |    0.01 | 0.0008 |      40 B |
|  UpdateMemoryCache |                 abcd | 144.54 ns | 0.325 ns | 0.304 ns | 10.66 |    0.05 | 0.0043 |     224 B |
| UpdateCacheManager |                 abcd | 113.50 ns | 0.297 ns | 0.278 ns |  8.37 |    0.04 | 0.0031 |     160 B |
|    UpdateLazyCache |                 abcd | 199.05 ns | 0.420 ns | 0.328 ns | 14.68 |    0.07 | 0.0093 |     480 B |
|                    |                      |           |          |          |       |         |        |           |
|       **TryGetCached** | **long (...)RCASE [50]** |  **53.75 ns** | **0.158 ns** | **0.147 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache | long (...)RCASE [50] |  64.36 ns | 0.109 ns | 0.091 ns |  1.20 |    0.00 |      - |         - |
| TryGetCacheManager | long (...)RCASE [50] |  65.06 ns | 0.083 ns | 0.078 ns |  1.21 |    0.00 |      - |         - |
|    TryGetLazyCache | long (...)RCASE [50] |  75.46 ns | 0.132 ns | 0.117 ns |  1.40 |    0.00 |      - |         - |
|       UpdateCached | long (...)RCASE [50] |  53.92 ns | 0.110 ns | 0.098 ns |  1.00 |    0.00 | 0.0007 |      40 B |
|  UpdateMemoryCache | long (...)RCASE [50] | 197.91 ns | 3.164 ns | 2.959 ns |  3.68 |    0.06 | 0.0043 |     224 B |
| UpdateCacheManager | long (...)RCASE [50] | 150.44 ns | 0.225 ns | 0.188 ns |  2.80 |    0.01 | 0.0029 |     160 B |
|    UpdateLazyCache | long (...)RCASE [50] | 256.51 ns | 0.275 ns | 0.230 ns |  4.78 |    0.01 | 0.0091 |     480 B |
