# FastCache.Cached
<img src="https://raw.githubusercontent.com/neon-sunset/fast-cache/main/src/FastCache.Cached/Assets/cached-transparent.png" width="180" height="180" align="right" />

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

## Access / Store latency and cost at throughput saturation
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET 6.0.5 (6.0.522.21309), X64 RyuJIT
```
|                Method |      Mean |    Error |    StdDev |    Median | Ratio |  Gen 0 | Allocated |
|---------------------- |----------:|---------:|----------:|----------:|------:|-------:|----------:|
| Get: FastCache.Cached |  15.63 ns | 0.452 ns |  1.334 ns |  14.61 ns |  1.00 |      - |         - |
| Get: MemoryCache      |  56.93 ns | 1.179 ns |  1.904 ns |  55.73 ns |  3.68 |      - |         - |
| Get: CacheManager     |  87.54 ns | 1.751 ns |  2.454 ns |  89.32 ns |  5.68 |      - |         - |
| Get: LazyCache        |  73.43 ns | 1.216 ns |  1.138 ns |  73.25 ns |  4.71 |      - |         - |
| Add/Upd: FC.Cached    |  33.75 ns | 0.861 ns |  2.539 ns |  31.92 ns |  2.18 | 0.0024 |      40 B |
| Add/Upd: MemoryCache  | 203.32 ns | 4.033 ns |  6.956 ns | 199.77 ns | 13.23 | 0.0134 |     224 B |
| Add/Upd: CacheManager | 436.85 ns | 8.729 ns | 19.160 ns | 433.97 ns | 28.10 | 0.0215 |     360 B |
| Add/Upd: LazyCache    | 271.56 ns | 5.428 ns |  7.785 ns | 274.19 ns | 17.58 | 0.0286 |     480 B |

Further reading "Keys and composite keys performance estimation": **[Code](src/FastCache.Benchmarks/Defaults.cs)** / **[Results](docs/full-api-approx-perf-estimation-net7.md)**

### Notes
- FastCache.Cached defaults provide highest performance and don't require from a developer to spend time on finding a way to use API optimally.
- Comparison was made with a string-based key. Composite keys supported by FastCache.Cached have additional performance cost.
- `CacheManger` documentation suggests using `WithMicrosoftMemoryCacheHandle()` by default which has terrible performance (and uses obsolete `System.Runtime.Caching`). We give it a better fighting chance by using `WithDictionaryHandle()` instead.
- Overall performance stays relatively comparable when downgrading to .NET 5 and decreases further by 15-30% when using .NET Core 3.1 with the difference ratio between libraries staying close to provided above.
- Non-standard platforms (the ones that aren't CLR based) use DateTime.UtcNow fallback instead of Environment.TickCount64, which will perform slower depending on the platform-specific implementation.
### On benchmark data
Throughput saturation means that all necessary data structures are fully available in the CPU cache and branch predictor has learned branch patters of the executed code.
This is only possible in scenarios such as items being retrieved or added/updated in a tight loop or very frequently on the same cores.
This means that real world performance will not saturate maximum throughput and will be bottlenecked by memory access latency and branch misprediction stalls.
As a result, you can expect resulting performance variance of 1-10x min latency depending on hardware and outside factors.
