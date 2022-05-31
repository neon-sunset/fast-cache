namespace FastCache;

public static class Cached<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K>(K key, out Cached<K, V> cached) where K : notnull
    {
        return TryGetInternal(key, out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2>(K1 param1, K2 param2, out Cached<ValueTuple<K1, K2>, V> cached)
    {
        return TryGetInternal((param1, param2), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2, K3>(K1 param1, K2 param2, K3 param3, out Cached<ValueTuple<K1, K2, K3>, V> cached)
    {
        return TryGetInternal((param1, param2, param3), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2, K3, K4>(K1 param1, K2 param2, K3 param3, K4 param4, out Cached<ValueTuple<K1, K2, K3, K4>, V> cached)
    {
        return TryGetInternal((param1, param2, param3, param4), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2, K3, K4, K5>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, out Cached<ValueTuple<K1, K2, K3, K4, K5>, V> cached)
    {
        return TryGetInternal((param1, param2, param3, param4, param5), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2, K3, K4, K5, K6>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, out Cached<ValueTuple<K1, K2, K3, K4, K5, K6>, V> cached)
    {
        return TryGetInternal((param1, param2, param3, param4, param5, param6), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<K1, K2, K3, K4, K5, K6, K7>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, K7 param7, out Cached<ValueTuple<K1, K2, K3, K4, K5, K6, K7>, V> cached)
    {
        return TryGetInternal((param1, param2, param3, param4, param5, param6, param7), out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal<K>(K key, out Cached<K, V> cached) where K : notnull
    {
        var found = CacheStaticHolder<K, V>
            .s_store
            .TryGetValue(key, out var inner);

        cached = new(key, inner.Value, found);
        return found && inner.IsNotExpired();
    }
}
