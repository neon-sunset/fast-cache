# FastCache.Cached
<p><img src="https://raw.githubusercontent.com/neon-sunset/fast-cache/main/img/cached-small-transparent.png" width="180" height="180" align="right" /></p>

[![CI/CD](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml/badge.svg)](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml)
[![nuget](https://badgen.net/nuget/v/FastCache.Cached/latest)](https://www.nuget.org/packages/FastCache.Cached/)

High-performance, thread-safe and easy to use cache for items with set expiration time.

Optimized for both dozens and millions of items. Features lock-free reads and writes, allocation-free reads, low memory footprint per item and automatic eviction.

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
### Read/Write lowest achievable latency
|                Method |      Mean |    Error |    StdDev |    Median | Ratio |  Gen 0 | Allocated |
|---------------------- |----------:|---------:|----------:|----------:|------:|-------:|----------:|
| Get: FastCache.Cached |  15.63 ns | 0.452 ns |  1.334 ns |  14.61 ns |  1.00 |      - |         - |
| Get: MemoryCache      |  56.93 ns | 1.179 ns |  1.904 ns |  55.73 ns |  3.68 |      - |         - |
| Get: CacheManager*    |  87.54 ns | 1.751 ns |  2.454 ns |  89.32 ns |  5.68 |      - |         - |
| Get: LazyCache        |  73.43 ns | 1.216 ns |  1.138 ns |  73.25 ns |  4.71 |      - |         - |
| Add/Upd: FC.Cached    |  33.75 ns | 0.861 ns |  2.539 ns |  31.92 ns |  2.18 | 0.0024 |      40 B |
| Add/Upd: MemoryCache  | 203.32 ns | 4.033 ns |  6.956 ns | 199.77 ns | 13.23 | 0.0134 |     224 B |
| Add/Upd: CacheManager*| 436.85 ns | 8.729 ns | 19.160 ns | 433.97 ns | 28.10 | 0.0215 |     360 B |
| Add/Upd: LazyCache    | 271.56 ns | 5.428 ns |  7.785 ns | 274.19 ns | 17.58 | 0.0286 |     480 B |


### Memory cost and write throughput (incl. runtime region/segment allocation and cache store resizing latency overhead)
|                 Method |     Length | Writes/1s |         Mean |     StdDev | Ratio |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|----------------------- |----------- |-----------|-------------:|-----------:|------:|----------:|----------:|----------:|----------:|
| Save(MT): FC.CRange**  |  1,000,000 |    58.75M |     17.02 ms |   0.349 ms |  1.00 |   31.2500 |         - |         - |     53 MB |
| Save(ST): FC.CRange*** |  1,000,000 |    10.01M |     99.84 ms |   2.243 ms |  5.85 |         - |         - |         - |     53 MB |
| Save(ST): MemoryCache  |  1,000,000 |     3.72M |    268.41 ms |   8.688 ms | 15.77 |         - |         - |         - |    229 MB |
| Save(ST): CacheManager |  1,000,000 |     2.84M |    351.90 ms |  36.296 ms | 20.77 |         - |         - |         - |    168 MB |
| Save(ST): LazyCache    |  1,000,000 |     2.64M |    378.66 ms |  23.405 ms | 22.33 |         - |         - |         - |    473 MB |
|                        |            |           |              |            |       |           |           |           |           |
| Save(MT): FC.CRange**  | 10,000,000 |    35.39M |    282.53 ms | 119.807 ms |  1.00 |         - |         - |         - |    534 MB |
| Save(ST): FC.CRange*** | 10,000,000 |     7.23M |  1,381.61 ms | 100.710 ms |  5.68 |         - |         - |         - |    534 MB |
| Save(ST): MemoryCache  | 10,000,000 |     2.41M |  4,135.14 ms | 142.515 ms | 16.69 | 2000.0000 | 1000.0000 | 1000.0000 |  2,289 MB |
| Save(ST): CacheManager | 10,000,000 |     1.96M |  5,081.21 ms | 217.463 ms | 21.62 | 2000.0000 | 1000.0000 | 1000.0000 |  1,678 MB |
| Save(ST): LazyCache    | 10,000,000 |     1.82M |  5,467.67 ms | 192.579 ms | 23.09 | 3000.0000 | 2000.0000 | 1000.0000 |  4,730 MB |
|                        |            |           |              |            |       |           |           |           |           |
| Save(MT): FC.CRange**  | 20,000,000 |    26.77M |    746.86 ms |  10.026 ms |  1.00 |         - |         - |         - |  1,068 MB |
| Save(ST): FC.CRange*** | 20,000,000 |     6.11M |  3,269.26 ms |  69.772 ms |  4.38 |         - |         - |         - |  1,068 MB |
| Save(ST): MemoryCache  | 20,000,000 |     2.13M |  9,362.04 ms | 343.589 ms | 12.54 | 3000.0000 | 2000.0000 | 1000.0000 |  4,578 MB |
| Save(ST): CacheManager | 20,000,000 |     1.55M | 12,860.89 ms | 423.876 ms | 17.25 | 3000.0000 | 2000.0000 | 1000.0000 |  3,815 MB |
| Save(ST): LazyCache    | 20,000,000 |     1.71M | 11,690.66 ms | 484.762 ms | 15.69 | 5000.0000 | 2000.0000 | 1000.0000 |  9,460 MB |

Further reading "Keys and composite keys performance estimation": **[Code](src/FastCache.Benchmarks/Defaults.cs)** / **[Results](docs/full-api-approx-perf-estimation-net7.md)**

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