using NonBlocking;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_store = new();
    internal static readonly EvictionJob<T> s_evictionJob = new();
    internal static readonly EvictionQuickList<T> s_quickList = new();
}
