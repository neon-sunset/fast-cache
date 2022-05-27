# FastCache.Cached
[![CI/CD](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml/badge.svg)](https://github.com/neon-sunset/fast-cache/actions/workflows/dotnet-releaser.yml)
[![nuget](https://badgen.net/nuget/v/FastCache.Cached/latest)](https://www.nuget.org/packages/FastCache.Cached/)

High-performance, thread-safe and simple to use caching library that scales with ease from tens to tens of millions of items.
Features include automatic eviction, lock-free and wait-free access and storage, allocation-free access and low memory footprint.
Credit and thanks to Vladimir Sadov for his implementation of NonBlocking.ConcurrentDictionary which is used as a backing store.

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

Wrap a regular method call
```csharp
var report = Cached.GetOrCompute(month, year, GetReport, TimeSpan.FromMinutes(60));
```

Or an async one
```csharp
// For methods that return Task<T> or ValueTask<T>
var report = await Cached.GetOrCompute(month, year, GetReportAsync, TimeSpan.FromMinute(60));
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

Store common type (string) in a shared cache store (OK for small (<1M) to mid (<5M) sized collections)
```csharp
// GetOrCompute<T...> where T is string
var userNote = Cached.GetOrCompute(userId, GetUserNoteString, TimeSpan.FromMinutes(5));
```

Or in a separate one by using value object (Recommended)
```csharp
readonly record struct UserNote(string Value);

// GetOrCompute<T...> where T is UserNote
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
- High performance and scaling covering both simplest applications and highly loaded services. Can handle 1-100M+ items with O(1) access/storage time and O(n~) memory cost/cpu time cost for full eviction
- Lock-free and wait-free access and storage of cache items. Performance will scale with threads, data synchronization cost is minimal thanks to 'NonBlocking.ConcurrentDictionary' backing store by Vladimir Sadov
- Multi-key and composite-key store access without collisions between key types. Collisions are avoided by dispering type hashcode with value hashcode to compose cache item key ('int')
- Handles timezone/dst updates on most platforms by relying on system uptime timestamp for item expiration - `Environment.TickCount64` which is also significantly faster than `DateTime.UtcNow`

## Access / Store latency and cost at throughput saturation
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET 6.0.5 (6.0.522.21309), X64 RyuJIT
```
|                Method |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|---------------------- |------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Get: FastCache.Cached |    15.92 ns |  0.367 ns |  0.941 ns |    15.31 ns |      - |      - |         - |
| Get: MemoryCache      |    86.74 ns |  2.227 ns |  6.565 ns |    89.43 ns | 0.0019 |      - |      32 B |
| Get: CacheManager     |   167.03 ns |  3.395 ns |  9.002 ns |   162.56 ns | 0.0105 |      - |     176 B |
| Get: LazyCache        |    74.46 ns |  1.510 ns |  2.214 ns |    74.81 ns |      - |      - |         - |
| Add/Upd: FC.Cached    |    34.57 ns |  0.920 ns |  2.711 ns |    33.73 ns | 0.0024 |      - |      40 B |
| Add/Upd: MemoryCache  |   778.21 ns | 16.728 ns | 49.060 ns |   775.08 ns | 0.4082 | 0.0038 |   6,832 B |
| Add/Upd: CacheManager | 1,052.22 ns | 20.926 ns | 27.209 ns | 1,053.61 ns | 0.0744 |      - |   1,248 B |
| Add/Upd: LazyCache    |   281.60 ns |  3.984 ns |  3.532 ns |   281.79 ns | 0.0286 |      - |     480 B |
### Notes
- *FastCache.Cached add and update operations are represented by single `cached.Save(param1...param7, expiration)` which will either add or replace existing value updating its expiration*
- *Comparison was made with a string-based key. Composite keys supported by FastCache.Cached have significant performance cost if they have reference types which incurs 30-40ns extra cpu cost per each reference typed param*
- *CacheManager library provides methods with highly inconsistent performance and allocation characteristics. The method for it was chosen on the basis of closest functionality to 'non-throwing add or update'*
- *Overall performance stays relatively comparable when downgrading to .NET 5 and decreases further by 15-30% when using .NET Core 3.1 with the difference ratio between libraries staying close to provided above*
- *Non-standard platforms (the ones that aren't CLR based) use DateTime.UtcNow fallback instead of Environment.TickCount64, which will perform slower depending on the platform-specific implementation*
### On benchmark data
Throughput saturation means that all necessary data structures are fully available in the CPU cache and branch predictor has learned branch patters of the executed code.
This is only possible in scenarios such as items being retrieved or added/updated in a tight loop or very frequently on the same cores.
This means that real world performance will not saturate maximum throughput and will be bottlenecked by memory access latency and branch misprediction stalls.
As a result, you can expect resulting performance variance of 1-10x min latency depending on hardware and outside factors.
