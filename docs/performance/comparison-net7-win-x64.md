``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-rc.2.22419.24
  [Host]     : .NET 7.0.0 (7.0.22.41112), X64 RyuJIT
  DefaultJob : .NET 7.0.0 (7.0.22.41112), X64 RyuJIT


```
|             Method |              ItemKey |      Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|------------------- |--------------------- |----------:|---------:|---------:|------:|--------:|-------:|----------:|
|       **TryGetCached** |                    **A** |  **11.46 ns** | **0.160 ns** | **0.149 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache |                    A |  52.91 ns | 0.452 ns | 0.423 ns |  4.62 |    0.05 |      - |         - |
| TryGetCacheManager |                    A |  83.57 ns | 1.715 ns | 2.865 ns |  7.33 |    0.28 |      - |         - |
|    TryGetLazyCache |                    A |  68.79 ns | 1.427 ns | 1.335 ns |  6.00 |    0.15 |      - |         - |
|       UpdateCached |                    A |  29.54 ns | 0.614 ns | 1.721 ns |  2.56 |    0.17 | 0.0001 |      40 B |
|  UpdateMemoryCache |                    A | 169.38 ns | 3.218 ns | 3.304 ns | 14.78 |    0.33 | 0.0007 |     224 B |
| UpdateCacheManager |                    A | 181.26 ns | 2.887 ns | 2.559 ns | 15.80 |    0.21 | 0.0005 |     160 B |
|    UpdateLazyCache |                    A | 286.95 ns | 2.920 ns | 2.732 ns | 25.04 |    0.41 | 0.0014 |     480 B |
|                    |                      |           |          |          |       |         |        |           |
|       **TryGetCached** |                 **abcd** |  **12.64 ns** | **0.297 ns** | **0.292 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache |                 abcd |  51.99 ns | 0.799 ns | 0.748 ns |  4.11 |    0.07 |      - |         - |
| TryGetCacheManager |                 abcd |  84.88 ns | 1.748 ns | 3.240 ns |  6.71 |    0.27 |      - |         - |
|    TryGetLazyCache |                 abcd |  69.80 ns | 0.727 ns | 0.680 ns |  5.51 |    0.15 |      - |         - |
|       UpdateCached |                 abcd |  30.04 ns | 0.623 ns | 1.585 ns |  2.37 |    0.14 | 0.0001 |      40 B |
|  UpdateMemoryCache |                 abcd | 173.13 ns | 3.243 ns | 3.034 ns | 13.68 |    0.44 | 0.0007 |     224 B |
| UpdateCacheManager |                 abcd | 185.04 ns | 3.267 ns | 3.056 ns | 14.62 |    0.38 | 0.0005 |     160 B |
|    UpdateLazyCache |                 abcd | 281.62 ns | 2.612 ns | 2.443 ns | 22.25 |    0.49 | 0.0014 |     480 B |
|                    |                      |           |          |          |       |         |        |           |
|       **TryGetCached** | **long (...)RCASE [50]** |  **33.86 ns** | **0.693 ns** | **0.901 ns** |  **1.00** |    **0.00** |      **-** |         **-** |
|  TryGetMemoryCache | long (...)RCASE [50] |  76.81 ns | 1.532 ns | 1.433 ns |  2.26 |    0.07 |      - |         - |
| TryGetCacheManager | long (...)RCASE [50] | 107.33 ns | 1.924 ns | 1.800 ns |  3.16 |    0.13 |      - |         - |
|    TryGetLazyCache | long (...)RCASE [50] |  93.76 ns | 1.790 ns | 1.674 ns |  2.76 |    0.11 |      - |         - |
|       UpdateCached | long (...)RCASE [50] |  51.36 ns | 1.019 ns | 1.645 ns |  1.52 |    0.06 | 0.0001 |      40 B |
|  UpdateMemoryCache | long (...)RCASE [50] | 219.22 ns | 3.552 ns | 3.323 ns |  6.46 |    0.22 | 0.0007 |     224 B |
| UpdateCacheManager | long (...)RCASE [50] | 206.63 ns | 3.058 ns | 2.860 ns |  6.09 |    0.21 | 0.0005 |     160 B |
|    UpdateLazyCache | long (...)RCASE [50] | 330.30 ns | 3.280 ns | 2.908 ns |  9.75 |    0.29 | 0.0014 |     480 B |
