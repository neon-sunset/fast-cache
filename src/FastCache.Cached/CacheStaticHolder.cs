using FastCache.Collections;
using FastCache.Extensions;
using FastCache.Helpers;
using FastCache.Services;
using NonBlocking;

namespace FastCache;

#if NET5_0_OR_GREATER
#pragma warning disable CA2255 // Static initialization to ensure cctor checks are removed in JITted code
internal static class CacheStaticHolder
{
    [ModuleInitializer]
    public static void Initialize()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Constants).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CacheStaticHolder<,>).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CacheManager).TypeHandle);

        RuntimeHelpers.RunClassConstructor(typeof(Cached<,>).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(Cached).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CachedExtensions).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CachedRange).TypeHandle);

        RuntimeHelpers.RunClassConstructor(typeof(TimeUtils).TypeHandle);
    }
}
#pragma warning restore CA2255
#endif

internal static class CacheStaticHolder<K, V> where K : notnull
{
    internal static readonly ConcurrentDictionary<K, CachedInner<V>> Store = new();
    internal static readonly EvictionJob<K, V> EvictionJob = new();
    internal static readonly EvictionQuickList<K, V> QuickList = new();
}
