# FastCache.Cached
<p><img src="https://raw.githubusercontent.com/neon-sunset/fast-cache/main/img/cached-small-transparent.png" width="180" height="180" align="right" /></p>

[![CI/CD](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml/badge.svg)](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml)
[![nuget](https://badgen.net/nuget/v/FastCache.Cached/latest)](https://www.nuget.org/packages/FastCache.Cached/)

The fastest cache written in C# for items with set expiration time. Easy to use, thread-safe and light on memory.

Optimized to scale from dozens to millions of items. Features lock-free reads and writes, allocation-free reads and automatic eviction.

Credit to Vladimir Sadov for his implementation of `NonBlocking.ConcurrentDictionary` which is used as an underlying store.

## Quick start
`dotnet add package FastCache.Cached` or `Install-Package FastCache.Cached`

Get cached value or save a new one with expiration of 60 minutes
```csharp
public FinancialReport GetReport(int month, int year)
{
  if (Cached<FinancialReport>.TryGet(month, year, out var cached))
  {
    return cached.Value;
  }

  var report = // Expensive computation: retrieve data and calculate report

  return cached.Save(report, TimeSpan.FromMinutes(60));
}
```

Wrap and cache the result of a regular method call
```csharp
var report = Cached.GetOrCompute(month, year, GetReport, TimeSpan.FromMinutes(60));
```

Or an async one
```csharp
// For methods that return Task<T> or ValueTask<T>
var report = await Cached.GetOrCompute(month, year, GetReportAsync, TimeSpan.FromMinutes(60));
```

Save the value to cache but only if the cache size is below limit
```csharp
public FinancialReport GetReport(int month, int year)
{
  if (Cached<FinancialReport>.TryGet(month, year, out var cached))
  {
    return cached.Value;
  }

  return cached.Save(report, TimeSpan.FromMinutes(60), limit: 2_500_000);
}
```
```csharp
// GetOrCompute with maximum cache size limit.
// RAM is usually plenty but what if the user runs Chrome?
var report = Cached.GetOrCompute(month, year, GetReport, TimeSpan.FromMinutes(60), limit: 2_500_000);
```

Add new data without accessing cache item first (e.g. loading a large batch of independent values to cache)
```csharp
using FastCache.Extensions;
...
foreach (var ((month, year), report) in reportsResultBatch)
{
  report.Cache(month, year, TimeSpan.FromMinutes(60));
}
```

Store common type (string) in a shared cache store (other users may share the cache for the same parameter type, this time it's `int`)
```csharp
// GetOrCompute<...V> where V is string.
// To save some other string for the same 'int' number simultaneously, look at the option below :)
var userNote = Cached.GetOrCompute(userId, GetUserNoteString, TimeSpan.FromMinutes(5));
```

Or in a separate one by using value object (Recommended)
```csharp
readonly record struct UserNote(string Value);

// GetOrCompute<...V> where V is UserNote
var userNote = Cached.GetOrCompute(userId, GetUserNote, TimeSpan.FromMinutes(5));
```
```csharp
// This is how it looks for TryGet
if (Cached<UserNote>.TryGet(userId, out var cached))
{
  return cached.Value;
}
...
return cached.Save(userNote, TimeSpan.FromMinutes(5));
```

## Features and design philosophy
- In-memory cache for items with expiration time and automatic eviction
- Little to no ceremony - no need to configure or initialize, just add the package and you are ready to go. Behavior can be further customized via env variables
- Focused design allows to reduce memory footprint per item and minimize overhead via inlining and static dispatch
- High performance and scaling covering both simplest applications and highly loaded services. Can handle 1-100M+ items with O(1) read/write time and up to O(n~) memory cost/cpu time cost for full eviction
- Lock-free and wait-free reads/writes of cached items. Performance will improve with threads, data synchronization cost is minimal thanks to [NonBlocking.ConcurrentDictionary](https://github.com/VSadov/NonBlocking)
- Multi-key store access without collisions between key types. Collisions are avoided by statically dispatching on the composite key type signature e.g. `(string, CustomEnum, int)` together with the type of cached value - composite keys are structurally evaluated for equality, different combinations will correspond to different cache items
- Handles timezone/DST updates on most platforms by relying on system uptime timestamp for item expiration - `Environment.TickCount64` which is also significantly faster than `DateTime.UtcNow`

## Performance
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET 6.0.5 (6.0.522.21309), X64 RyuJIT
```
### TLDR: `FastCache.Cached` vs `Microsoft.Extensions.Caching.Memory.MemoryCache`
|            Library | Lowest read latency | Read throughput (M/1s) | Lowest write latency | Write throughput (M/1s) | Cost per item | Cost per 10M items |
| ------------------ | ------------------- | ---------------------- | -------------------- | ----------------------- | ------------- | ------------------ |
|   **FastCache.Cached** |            **15.63 ns** | **114-288M MT / 9-72M ST** |             **33.75 ns** |    **39-81M MT / 6-31M ST** |          **40 B** |             **381 MB** |
|        MemoryCache |            56.93 ns |   41-46M MT / 4-10M ST |            203.32 ns |    11-26M MT /  2-6M ST |         224 B |           2,136 MB |
|       CacheManager |            87.54 ns |                    N/A |           ~436.85 ns |      N/A MT / 1.5-5M ST | (+alloc)360 B |           1,602 MB |


+`CachedRange.Save(ReadOnlySpan<(K, V)>)` provides parallelized bulk writes out of box

++`CacheManager` doesn't have read throughput results because test suite would take too long to run to include `CacheManager` and `LazyCache`. Given higher CPU usage by `CacheManager` and higher RAM usage by `LazyCache` it is reasonable to assume they would score lower and scale worse due to higher number of locks

### Read/Write lowest achievable latency
|                Method |      Mean |    Error |    StdDev |    Median | Ratio |  Gen 0 | Allocated |
|---------------------- |----------:|---------:|----------:|----------:|------:|-------:|----------:|
| **Get: FastCache.Cached** |  **15.63 ns** | **0.452 ns** |  **1.334 ns** |  **14.61 ns** |  **1.00** |      **-** |         **-** |
| Get: MemoryCache      |  56.93 ns | 1.179 ns |  1.904 ns |  55.73 ns |  3.68 |      - |         - |
| Get: CacheManager*    |  87.54 ns | 1.751 ns |  2.454 ns |  89.32 ns |  5.68 |      - |         - |
| Get: LazyCache        |  73.43 ns | 1.216 ns |  1.138 ns |  73.25 ns |  4.71 |      - |         - |
| **Set: FastCache.Cached** |  **33.75** ns | **0.861 ns** |  **2.539 ns** |  **31.92 ns** |  **2.18** | **0.0024** |      **40 B** |
| Set: MemoryCache      | 203.32 ns | 4.033 ns |  6.956 ns | 199.77 ns | 13.23 | 0.0134 |     224 B |
| Set: CacheManager*    | 436.85 ns | 8.729 ns | 19.160 ns | 433.97 ns | 28.10 | 0.0215 |     360 B |
| Set: LazyCache        | 271.56 ns | 5.428 ns |  7.785 ns | 274.19 ns | 17.58 | 0.0286 |     480 B |

### Read throughput detailed
|                Method |      Count | Reads/1s |             Mean |          Error |         StdDev | Ratio |
|---------------------- |----------- |--------- |-----------------:|---------------:|---------------:|------:|
| **Read(MT): FastCache**   |      **1,000** |  **130.97M** |        **7.635 us** |      **0.1223 us** |      **0.1144 us** |  **1.00** |
| Read(ST): FastCache   |      1,000 |   72.99M |        13.700 us |      0.2723 us |      0.5562 us |  1.78 |
| Read(MT): MemoryCache |      1,000 |   41.35M |        24.183 us |      1.2907 us |      3.7853 us |  2.68 |
| Read(ST): MemoryCache |      1,000 |   10.31M |        96.943 us |      0.9095 us |      0.8063 us | 12.71 |
|                       |            |          |                  |                |                |       |
| **Read(MT): FastCache**   |    **100,000** |  **288.66M** |       **346.418 us** |      **5.2196 us** |      **6.6011 us** |  **1.00** |
| Read(ST): FastCache   |    100,000 |   28.99M |     3,449.865 us |     66.4929 us |     81.6593 us |  9.96 |
| Read(MT): MemoryCache |    100,000 |   46.77M |     2,138.400 us |    175.2152 us |    516.6259 us |  6.32 |
| Read(ST): MemoryCache |    100,000 |    4.64M |    21,540.964 us |    394.9239 us |    499.4523 us | 62.20 |
|                       |            |          |                  |                |                |       |
| **Read(MT): FastCache**   |  **1,000,000** |  **114.54M** |     **8,730.009 us** |    **173.8538 us** |    **170.7476 us** |  **1.00** |
| Read(ST): FastCache   |  1,000,000 |    9.74M |   102,580.795 us |    926.3173 us |    866.4778 us | 11.76 |
| Read(MT): MemoryCache |  1,000,000 |   41.46M |    24,114.261 us |    369.3612 us |    308.4334 us |  2.76 |
| Read(ST): MemoryCache |  1,000,000 |    3.92M |   254,619.996 us |  2,585.3079 us |  2,291.8081 us | 29.17 |
|                       |            |          |                  |                |                |       |
| **Read(MT): FastCache**   | **10,000,000** |  **112.89M** |    **88,584.244** us |  **1,709.9078** us |  **1,599.4488** us |  **1.00** |
| Read(ST): FastCache   | 10,000,000 |    9.70M | 1,030,431.980 us |  9,874.4883 us |  9,236.6025 us | 11.64 |
| Read(MT): MemoryCache | 10,000,000 |   42.84M |   233,410.703 us |  2,945.8464 us |  2,299.9231 us |  2.63 |
| Read(ST): MemoryCache | 10,000,000 |    4.13M | 2,421,159.114 us | 35,280.8135 us | 31,275.5222 us | 27.33 |

Further reading
- Keys and composite keys performance estimation: **[Code](src/FastCache.Benchmarks/Defaults.cs)** / **[Results](docs/full-api-approx-perf-estimation-net7.md)**

#### Notes
- FastCache.Cached defaults provide highest performance and don't require spending time on finding a way to use API optimally. The design goal is to nudge a developer to use the fastest way to achieve his or her goals while strictly adhering to the principle of "pay for play".
- Comparison was made with a string-based key. Composite keys supported by FastCache.Cached have additional performance cost. Numeric keys have significantly higher performance in some instances (down to 5ns read latency on x86_64 where `K` is `int` or `uint` and `V` fits in a single register).
- *`CacheManger` documentation suggests using `WithMicrosoftMemoryCacheHandle()` by default which has terrible performance (and uses obsolete `System.Runtime.Caching`). We give it a better fighting chance by using `WithDictionaryHandle()` instead.
- **`CachedRange.Save(ReadOnlyMemory<(K, V)>)` (and `CachedRange.Save(ReadOnlyMemory<K>, ReadOnlyMemory<V>)`) automatically splits the range into slices of _1024 or length / cpu cores_ elements and saves them in parallel which dramatically improves throughput. This works especially well because the execution flow is lock-free and does not suffer from false sharing of CPU cache lines (except for the adjacent entries at the edges of slices)
- ***Forced single-threaded execution of `CachedRange.Save(ReadOnlyMemory<(K, V)>)`
- Overall, performance stays relatively comparable when downgrading to .NET 5 and decreases further by 15-30% when using .NET Core 3.1 with the difference ratio between libraries staying close to provided above.
- Non-standard platforms (the ones that aren't CLR based) use DateTime.UtcNow fallback instead of Environment.TickCount64, which will perform slower depending on the platform-specific implementation.
#### On benchmark data
Throughput saturation means that all necessary data structures are fully available in the CPU cache and branch predictor has learned branch patters of the executed code.
This is only possible in scenarios such as items being retrieved or added/updated in a tight loop or very frequently on the same cores.
This means that real world performance will not saturate maximum throughput and will be bottlenecked by memory access latency and branch misprediction stalls.
As a result, you can expect resulting performance variance of 1-10x min latency depending on hardware and outside factors.

---
### From 🇺🇦 Ukraine with ♥️