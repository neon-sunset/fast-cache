## FastCache.Cached
### Quick start
`dotnet add package FastCache.Cached` or `Install-Package FastCache.Cached`

*Recommended: to get optimal results, use `Cached<YourType>` per source instead of common types such as `string` shared across multiple sources.*
#### Get cached value or save a new one with expiration of 60 minutes
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
#### Wrap a regular method call
```csharp
var report = Cached.GetOrCompute(month, year, GetReport, TimeSpan.FromMinutes(60));
```
#### Or an async one
```csharp
// For methods that return Task<T> or ValueTask<T>
var report = await Cached.GetOrCompute(month, year, GetReportAsync, TimeSpan.FromMinute(60));
```
#### Add new data without accessing cache item first (e.g. loading a large batch of independent values to cache)
```csharp
using FastCache.Extensions;
...
foreach (var ((month, year), report) in reportsResultBatch)
{
  report.Cache(month, year, TimeSpan.FromMinutes(60));
}
```
#### Store common type (string) in a shared cache store (OK for small to mid sized collections)
```csharp
// GetOrCompute<T...> where T is string
var userNote = await Cached.GetOrCompute(userId, GetUserNoteString, TimeSpan.FromMinutes(5));
```
#### Or in a separate one by using value object (Recommended)
```csharp
readonly record struct UserNote(string Value);

// GetOrCompute<T...> where T is UserNote
var reportNote = await Cached.GetOrCompute(userId, GetUserNote, TimeSpan.FromMinutes(5));
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

### Features and design philosophy
- In-memory cache for items with expiration time and automatic eviction
- Easy to use - no need to configure or initialize, just `dotnet add package FastCache.Cached` and you are ready to go. Behavior can be further customized via env variables / appcontext switches (tentative)
- High performance: low access/store latency and high throughput
- Focused design allows to reduce memory footprint per item and minimize overhead via inlining and static dispatch
- Performance and scaling covering both simplest applications and highly loaded services. Can handle 1-100M+ items with O(1) access/storage time and O(n~) memory cost/cpu time cost for full eviction
- Lock-free and wait-free access and storage of cache items. Performance will scale with threads, data synchronization cost is minimal thanks to 'NonBlocking.ConcurrentDictionary' backing store by Vladimir Sadov
- Multi-key and composite-key store access without collisions between key types. Collisions are avoided by dispering type hashcode with value hashcode to compose cache item key ('int')
- Handles timezone/dst updates on most platforms by relying on system uptime timestamp for item expiration - `Environment.TickCount64` which is also significantly faster than `DateTime.UtcNow`

### Access / Store latency and cost at throughput saturation
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET 6.0.5 (6.0.522.21309), X64 RyuJIT
```
|             Method           |      Mean |    Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|----------------------------- |----------:|---------:|----------:|----------:|------:|--------:|-------:|----------:|
| Get: FastCache.Cached        |  13.39 ns | 0.355 ns |  0.552 ns |  13.47 ns |  1.00 |    0.00 |      - |         - |
| Get: LazyCache               |  54.87 ns | 1.059 ns |  2.015 ns |  54.56 ns |  4.09 |    0.20 |      - |         - |
| Get: CacheManager(In-memory) | 147.85 ns | 3.028 ns |  6.047 ns | 147.36 ns | 11.06 |    0.71 | 0.0105 |     176 B |
| Add/Update: FastCache.Cached |  33.74 ns | 1.203 ns |  3.548 ns |  32.65 ns |  2.58 |    0.23 | 0.0024 |      40 B |
| Add/Update: LazyCache        | 198.92 ns | 5.406 ns | 15.940 ns | 193.24 ns | 15.05 |    1.22 | 0.0286 |     480 B |
| Upd: CacheManager(In-memory) | 105.79 ns | 2.405 ns |  7.091 ns | 106.26 ns |  7.79 |    0.68 | 0.0176 |     296 B |

*FastCache.Cached add and update operations are represented by single `cached.Save(param1...param8, expiration)` which will either add or replace existing value updating its expiration*
### On benchmark data
Throughput saturation means that all necessary data structures are fully available in the CPU cache and branch predictor has learned branch patters of the executed code.
This is only possible in scenarios such as items being retrieved or added/updated in a tight loop or very frequently on the same cores.
This means that real world performance will not saturate maximum throughput and will be bottlenecked by memory access latency and branch misprediction stalls.
As a result, you can expect resulting performance variance of 1-10x min latency depending on hardware and outside factors.
