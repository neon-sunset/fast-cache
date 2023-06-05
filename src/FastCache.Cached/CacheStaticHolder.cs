using NonBlocking;

namespace FastCache;

internal static class CacheStaticHolder<K, V> where K : notnull
{
    internal static readonly ConcurrentDictionary<K, CachedInner<V>> Store = new();
    internal static readonly EvictionJob<K, V> EvictionJob = new();
    internal static readonly EvictionQuickList<K, V> QuickList = new();
}
