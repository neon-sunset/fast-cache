using NonBlocking;

namespace FastCache;

internal static class CacheStaticHolder<K, V> where K : notnull
{
    internal static readonly ConcurrentDictionary<K, CachedInner<V>> s_store = new();
    internal static readonly EvictionJob<K, V> s_evictionJob = new();
    internal static readonly EvictionQuickList<K, V> s_quickList = new();
}
