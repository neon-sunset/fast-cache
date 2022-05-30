``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.4 (21F79) [Darwin 21.5.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=7.0.100-preview.6.22276.5
  [Host]   : .NET 7.0.0 (7.0.22.27203), Arm64 RyuJIT
  ShortRun : .NET 7.0.0 (7.0.22.27203), Arm64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
|                Method |      Mean |     Error |   StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |----------:|----------:|---------:|------:|--------:|-------:|----------:|
|          TryGetSingle |  15.06 ns |  0.290 ns | 0.016 ns |  1.00 |    0.00 |      - |         - |
|             TryGetTwo |  36.33 ns |  0.324 ns | 0.018 ns |  2.41 |    0.00 |      - |         - |
|           TryGetThree |  58.23 ns |  3.408 ns | 0.187 ns |  3.87 |    0.01 |      - |         - |
|            TryGetFour |  91.96 ns |  2.008 ns | 0.110 ns |  6.10 |    0.01 |      - |         - |
|           TryGetSeven | 136.24 ns |  3.563 ns | 0.195 ns |  9.04 |    0.02 |      - |         - |
|       TryGetSingleInt |  13.79 ns |  0.192 ns | 0.010 ns |  0.92 |    0.00 |      - |         - |
|          TryGetTwoInt |  17.25 ns |  0.538 ns | 0.029 ns |  1.15 |    0.00 |      - |         - |
|        TryGetThreeInt |  47.34 ns |  1.120 ns | 0.061 ns |  3.14 |    0.00 |      - |         - |
|         TryGetFourInt |  23.75 ns |  0.400 ns | 0.022 ns |  1.58 |    0.00 |      - |         - |
|        TryGetSevenInt |  28.51 ns |  3.519 ns | 0.193 ns |  1.89 |    0.01 |      - |         - |
|        TryGetTwoMixed |  34.65 ns |  1.267 ns | 0.069 ns |  2.30 |    0.01 |      - |         - |
|      TryGetThreeMixed |  55.99 ns |  3.124 ns | 0.171 ns |  3.72 |    0.01 |      - |         - |
|       TryGetFourMixed |  58.51 ns | 21.793 ns | 1.195 ns |  3.88 |    0.08 |      - |         - |
|      TryGetSevenMixed | 100.53 ns |  5.505 ns | 0.302 ns |  6.67 |    0.02 |      - |         - |
|      GetAndSaveSingle |  75.71 ns |  1.551 ns | 0.085 ns |  5.03 |    0.01 | 0.0063 |      40 B |
|         GetAndSaveTwo | 131.41 ns |  3.247 ns | 0.178 ns |  8.72 |    0.01 | 0.0062 |      40 B |
|       GetAndSaveThree | 171.62 ns |  7.332 ns | 0.402 ns | 11.39 |    0.04 | 0.0062 |      40 B |
|        GetAndSaveFour | 204.99 ns |  7.144 ns | 0.392 ns | 13.61 |    0.01 | 0.0062 |      40 B |
|       GetAndSaveSeven | 286.19 ns | 15.466 ns | 0.848 ns | 19.00 |    0.05 | 0.0062 |      40 B |
|   GetAndSaveSingleInt |  63.26 ns |  3.861 ns | 0.212 ns |  4.20 |    0.01 | 0.0063 |      40 B |
|      GetAndSaveTwoInt |  74.23 ns |  1.361 ns | 0.075 ns |  4.93 |    0.01 | 0.0063 |      40 B |
|    GetAndSaveThreeInt |  76.30 ns |  8.626 ns | 0.473 ns |  5.06 |    0.04 | 0.0063 |      40 B |
|     GetAndSaveFourInt |  80.61 ns |  1.208 ns | 0.066 ns |  5.35 |    0.01 | 0.0063 |      40 B |
|    GetAndSaveSevenInt |  84.36 ns |  2.791 ns | 0.153 ns |  5.60 |    0.02 | 0.0063 |      40 B |
|    GetAndSaveTwoMixed | 131.43 ns |  4.726 ns | 0.259 ns |  8.72 |    0.02 | 0.0062 |      40 B |
|  GetAndSaveThreeMixed | 161.34 ns |  8.149 ns | 0.447 ns | 10.71 |    0.03 | 0.0062 |      40 B |
|   GetAndSaveFourMixed | 183.10 ns |  4.434 ns | 0.243 ns | 12.15 |    0.01 | 0.0062 |      40 B |
|  GetAndSaveSevenMixed | 241.23 ns |  9.946 ns | 0.545 ns | 16.01 |    0.03 | 0.0062 |      40 B |
|           CacheSingle |  58.43 ns |  3.627 ns | 0.199 ns |  3.88 |    0.02 | 0.0063 |      40 B |
|              CacheTwo |  80.64 ns |  1.792 ns | 0.098 ns |  5.35 |    0.01 | 0.0063 |      40 B |
|            CacheThree | 109.24 ns |  0.825 ns | 0.045 ns |  7.25 |    0.01 | 0.0063 |      40 B |
|             CacheFour | 129.69 ns |  6.188 ns | 0.339 ns |  8.61 |    0.02 | 0.0062 |      40 B |
|            CacheSeven | 174.66 ns |  7.005 ns | 0.384 ns | 11.59 |    0.02 | 0.0062 |      40 B |
|        CacheSingleInt |  53.82 ns |  9.511 ns | 0.521 ns |  3.57 |    0.04 | 0.0063 |      40 B |
|           CacheTwoInt |  57.55 ns |  1.616 ns | 0.089 ns |  3.82 |    0.01 | 0.0063 |      40 B |
|         CacheThreeInt |  56.53 ns |  4.024 ns | 0.221 ns |  3.75 |    0.02 | 0.0063 |      40 B |
|          CacheFourInt |  61.91 ns |  2.869 ns | 0.157 ns |  4.11 |    0.01 | 0.0063 |      40 B |
|         CacheSevenInt |  68.37 ns |  3.888 ns | 0.213 ns |  4.54 |    0.01 | 0.0063 |      40 B |
|         CacheTwoMixed |  76.60 ns |  1.624 ns | 0.089 ns |  5.08 |    0.01 | 0.0063 |      40 B |
|       CacheThreeMixed | 111.29 ns |  5.934 ns | 0.325 ns |  7.39 |    0.03 | 0.0063 |      40 B |
|        CacheFourMixed | 116.41 ns |  3.292 ns | 0.180 ns |  7.73 |    0.01 | 0.0063 |      40 B |
|       CacheSevenMixed | 140.18 ns |  0.544 ns | 0.030 ns |  9.31 |    0.01 | 0.0062 |      40 B |
|          GetOrCompute |  22.40 ns |  0.296 ns | 0.016 ns |  1.49 |    0.00 |      - |         - |
| GetOrComputeValueTask |  62.80 ns |  1.741 ns | 0.095 ns |  4.17 |    0.01 |      - |         - |
|      GetOrComputeTask |  70.77 ns |  1.977 ns | 0.108 ns |  4.70 |    0.00 | 0.0114 |      72 B |
