using FastCache.Helpers;

namespace FastCache.Extensions;

public static class CachedExtensions
{
    public static V Cache<K, V>(this V value, K key, TimeSpan expiration) where K : notnull
    {
        return CacheInternal(key, value, expiration);
    }

    public static V Cache<T1, T2, V>(this V value, T1 param1, T2 param2, TimeSpan expiration)
    {
        return CacheInternal((param1, param2), value, expiration);
    }

    public static V Cache<T1, T2, T3, V>(this V value, T1 param1, T2 param2, T3 param3, TimeSpan expiration)
    {
        return CacheInternal((param1, param2, param3), value, expiration);
    }

    public static V Cache<T1, T2, T3, T4, V>(this V value, T1 param1, T2 param2, T3 param3, T4 param4, TimeSpan expiration)
    {
        return CacheInternal((param1, param2, param3, param4), value, expiration);
    }

    public static V Cache<T1, T2, T3, T4, T5, V>(this V value, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, TimeSpan expiration)
    {
        return CacheInternal((param1, param2, param3, param4, param5), value, expiration);
    }

    public static V Cache<T1, T2, T3, T4, T5, T6, V>(this V value, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, TimeSpan expiration)
    {
        return CacheInternal((param1, param2, param3, param4, param5, param6), value, expiration);
    }

    public static V Cache<T1, T2, T3, T4, T5, T6, T7, V>(this V value, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, TimeSpan expiration)
    {
        return CacheInternal((param1, param2, param3, param4, param5, param6, param7), value, expiration);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static V CacheInternal<K, V>(K key, V value, TimeSpan expiration) where K : notnull
    {
        return new Cached<K, V>(key, default!, found: false).Save(value, expiration);
    }
}
