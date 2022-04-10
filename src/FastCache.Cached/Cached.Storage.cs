using System.Collections.Concurrent;
using FastCache.Jobs;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);

    // internal static readonly (Timer, T) s_removeExpiredJob = (
    //     new Timer(
    //         static _ => RemoveExpiredEntriesJob.Run<T>(),
    //         null,
    //         TimeSpan.FromSeconds(10),
    //         TimeSpan.FromSeconds(10)),
    //     default!);
}
