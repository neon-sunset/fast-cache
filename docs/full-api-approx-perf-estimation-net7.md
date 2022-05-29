## Preface
This data was gathered with `ShortRunJob` and is subject to high variance.
The benchmark was run to estimate approximate cost for the most probable params combinations.
It is planned to further research and, if possible, provide much more optimized composite keys code paths.
As of now, composite keys use ``ValueTuple`2`` and its implementations of `Equals` and `GetHashCode`.
It appears those perform better than just `readonly record struct`s.
As mentioned above, it is necessary to investigate if hand-rolled specialized implementations per each key combination perform better than `ValueTuple`s to produce better results.

## Rationale
Despite the drawbacks described above, it is an advantage to keep compsite keys impl. under the hood.
This way, the users don't have to worry about correct implementation when combining keys on their end (is string + bool.ToString() correct? what about other types?),
and aggregating reference types such as strings produces allocations on reading cache values which is undesirable.
Since it is common knowledge that types must implement proper `GetHashCode()` and `Equals()` to work as keys, no further constrains were placed on methods.
Doing otherwise would work against the goal of having as little ceremony as possible when using the library. This will be reconsidered with the arrival of "roles" and "extensions" (hopefully in .NET 7?).

In addition, other alternatives would force users to explicitly specify type signatures and would significantly clutter user code (especially `GetOrCompute`).

## Conclusion
Current approach is deemed appropriate to provide best balance between performance, usability and being foolproof while allowing for improvements in the future without breaking existing code.

## Data
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.5.22274.1
  [Host]   : .NET 7.0.0 (7.0.22.27203), X64 RyuJIT
  ShortRun : .NET 7.0.0 (7.0.22.27203), X64 RyuJIT

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
```
|                Method |       Mean |       Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------- |-----------:|------------:|----------:|------:|--------:|-------:|----------:|
|          TryGetSingle |  11.961 ns |  13.2702 ns | 0.7274 ns |  1.00 |    0.00 |      - |         - |
|             TryGetTwo |  46.261 ns |  26.8011 ns | 1.4691 ns |  3.88 |    0.28 |      - |         - |
|           TryGetThree |  58.493 ns |   0.7789 ns | 0.0427 ns |  4.90 |    0.30 |      - |         - |
|            TryGetFour |  73.868 ns |  51.6578 ns | 2.8315 ns |  6.19 |    0.47 |      - |         - |
|           TryGetSeven | 109.558 ns |   1.1329 ns | 0.0621 ns |  9.18 |    0.56 |      - |         - |
|       TryGetSingleInt |   4.213 ns |   0.4754 ns | 0.0261 ns |  0.35 |    0.02 |      - |         - |
|          TryGetTwoInt |  27.104 ns |  39.1513 ns | 2.1460 ns |  2.28 |    0.30 |      - |         - |
|        TryGetThreeInt |  37.473 ns |   0.1914 ns | 0.0105 ns |  3.14 |    0.19 |      - |         - |
|         TryGetFourInt |  31.386 ns |   0.3837 ns | 0.0210 ns |  2.63 |    0.16 |      - |         - |
|        TryGetSevenInt |  38.469 ns |   0.5389 ns | 0.0295 ns |  3.22 |    0.20 |      - |         - |
|        TryGetTwoMixed |  40.162 ns |   0.3690 ns | 0.0202 ns |  3.37 |    0.20 |      - |         - |
|      TryGetThreeMixed |  55.149 ns |   1.5484 ns | 0.0849 ns |  4.62 |    0.29 |      - |         - |
|       TryGetFourMixed |  53.275 ns |   0.1605 ns | 0.0088 ns |  4.47 |    0.27 |      - |         - |
|      TryGetSevenMixed |  80.585 ns |  22.8580 ns | 1.2529 ns |  6.75 |    0.43 |      - |         - |
|      GetAndSaveSingle |  40.910 ns |   1.7615 ns | 0.0966 ns |  3.43 |    0.22 | 0.0024 |      40 B |
|         GetAndSaveTwo | 104.097 ns |  11.0926 ns | 0.6080 ns |  8.72 |    0.53 | 0.0024 |      40 B |
|       GetAndSaveThree | 126.292 ns |   0.5798 ns | 0.0318 ns | 10.58 |    0.65 | 0.0024 |      40 B |
|        GetAndSaveFour | 155.695 ns | 112.9211 ns | 6.1896 ns | 13.04 |    0.72 | 0.0024 |      40 B |
|       GetAndSaveSeven | 230.388 ns |   5.9681 ns | 0.3271 ns | 19.31 |    1.19 | 0.0024 |      40 B |
|   GetAndSaveSingleInt |  26.982 ns |  61.3053 ns | 3.3603 ns |  2.27 |    0.40 | 0.0024 |      40 B |
|      GetAndSaveTwoInt |  65.446 ns |  12.8807 ns | 0.7060 ns |  5.48 |    0.34 | 0.0024 |      40 B |
|    GetAndSaveThreeInt |  77.114 ns |  68.9372 ns | 3.7787 ns |  6.46 |    0.50 | 0.0024 |      40 B |
|     GetAndSaveFourInt |  73.675 ns |  54.6800 ns | 2.9972 ns |  6.18 |    0.59 | 0.0024 |      40 B |
|    GetAndSaveSevenInt |  89.547 ns |   2.7708 ns | 0.1519 ns |  7.50 |    0.45 | 0.0024 |      40 B |
|    GetAndSaveTwoMixed |  88.153 ns |  24.6049 ns | 1.3487 ns |  7.39 |    0.48 | 0.0024 |      40 B |
|  GetAndSaveThreeMixed | 107.246 ns |   2.1456 ns | 0.1176 ns |  8.99 |    0.54 | 0.0024 |      40 B |
|   GetAndSaveFourMixed | 129.123 ns |   7.8736 ns | 0.4316 ns | 10.82 |    0.66 | 0.0024 |      40 B |
|  GetAndSaveSevenMixed | 184.798 ns | 153.6914 ns | 8.4243 ns | 15.49 |    1.25 | 0.0024 |      40 B |
|           CacheSingle |  30.262 ns |   0.1975 ns | 0.0108 ns |  2.54 |    0.16 | 0.0024 |      40 B |
|              CacheTwo |  67.272 ns |   3.5551 ns | 0.1949 ns |  5.64 |    0.36 | 0.0024 |      40 B |
|            CacheThree |  71.230 ns | 115.6780 ns | 6.3407 ns |  5.95 |    0.30 | 0.0024 |      40 B |
|             CacheFour |  93.204 ns |  90.9898 ns | 4.9875 ns |  7.81 |    0.55 | 0.0024 |      40 B |
|            CacheSeven | 124.537 ns |  22.0922 ns | 1.2109 ns | 10.43 |    0.54 | 0.0024 |      40 B |
|        CacheSingleInt |  25.161 ns |   1.0411 ns | 0.0571 ns |  2.11 |    0.13 | 0.0024 |      40 B |
|           CacheTwoInt |  41.423 ns | 105.6855 ns | 5.7930 ns |  3.47 |    0.55 | 0.0024 |      40 B |
|         CacheThreeInt |  53.303 ns |   1.1685 ns | 0.0640 ns |  4.47 |    0.28 | 0.0024 |      40 B |
|          CacheFourInt |  56.734 ns |   1.3267 ns | 0.0727 ns |  4.75 |    0.29 | 0.0024 |      40 B |
|         CacheSevenInt |  62.051 ns |   0.9014 ns | 0.0494 ns |  5.20 |    0.32 | 0.0024 |      40 B |
|         CacheTwoMixed |  58.261 ns |   2.4349 ns | 0.1335 ns |  4.88 |    0.29 | 0.0024 |      40 B |
|       CacheThreeMixed |  60.176 ns |   7.4671 ns | 0.4093 ns |  5.04 |    0.34 | 0.0024 |      40 B |
|        CacheFourMixed |  71.743 ns |  86.2051 ns | 4.7252 ns |  6.00 |    0.06 | 0.0024 |      40 B |
|       CacheSevenMixed | 106.937 ns |   1.8773 ns | 0.1029 ns |  8.96 |    0.54 | 0.0024 |      40 B |
|          GetOrCompute |  18.764 ns |   0.5802 ns | 0.0318 ns |  1.57 |    0.10 |      - |         - |
| GetOrComputeValueTask |  73.659 ns |  33.0612 ns | 1.8122 ns |  6.17 |    0.42 |      - |         - |
|      GetOrComputeTask |  62.821 ns |  31.0126 ns | 1.6999 ns |  5.26 |    0.18 | 0.0043 |      72 B |

