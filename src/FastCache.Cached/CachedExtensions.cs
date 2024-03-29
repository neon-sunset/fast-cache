namespace FastCache.Extensions;

public static class CachedExtensions
{
    public static V Cache<K, V>(this V value, K key, TimeSpan expiration) where K : notnull =>
        Cached<V>.Save(key, value, expiration);

    public static V Cache<K, V>(this V value, K key, TimeSpan expiration, uint limit) where K : notnull =>
        Cached<V>.Save(key, value, expiration, limit);

    public static V Cache<K1, K2, V>(this V value, K1 param1, K2 param2, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, value, expiration);

    public static V Cache<K1, K2, V>(this V value, K1 param1, K2 param2, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, value, expiration, limit);

    public static V Cache<K1, K2, K3, V>(this V value, K1 param1, K2 param2, K3 param3, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, param3, value, expiration);

    public static V Cache<K1, K2, K3, V>(this V value, K1 param1, K2 param2, K3 param3, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, param3, value, expiration, limit);

    public static V Cache<K1, K2, K3, K4, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, param3, param4, value, expiration);

    public static V Cache<K1, K2, K3, K4, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, param3, param4, value, expiration, limit);

    public static V Cache<K1, K2, K3, K4, K5, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, value, expiration);

    public static V Cache<K1, K2, K3, K4, K5, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, value, expiration, limit);

    public static V Cache<K1, K2, K3, K4, K5, K6, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, param6, value, expiration);

    public static V Cache<K1, K2, K3, K4, K5, K6, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, param6, value, expiration, limit);

    public static V Cache<K1, K2, K3, K4, K5, K6, K7, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, K7 param7, TimeSpan expiration) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, param6, param7, value, expiration);

    public static V Cache<K1, K2, K3, K4, K5, K6, K7, V>(this V value, K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, K7 param7, TimeSpan expiration, uint limit) =>
        Cached<V>.Save(param1, param2, param3, param4, param5, param6, param7, value, expiration, limit);

}
