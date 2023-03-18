namespace FastCache;

public static partial class Cached<V>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K>(K key, V value, TimeSpan expiration) where K : notnull =>
        SaveInternal(key, value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K>(K key, V value, TimeSpan expiration, uint limit) where K : notnull =>
        SaveInternal(key, value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2>(K1 param1, K2 param2, V value, TimeSpan expiration) =>
        SaveInternal((param1, param2), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2>(K1 param1, K2 param2, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3>(K1 param1, K2 param2, K3 param3, V value, TimeSpan expiration) =>
        SaveInternal((param1, param2, param3), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3>(K1 param1, K2 param2, K3 param3, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2, param3), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4>(K1 param1, K2 param2, K3 param3, K4 param4, V value, TimeSpan expiration) =>
    SaveInternal((param1, param2, param3, param4), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4>(K1 param1, K2 param2, K3 param3, K4 param4, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2, param3, param4), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, V value, TimeSpan expiration) =>
        SaveInternal((param1, param2, param3, param4, param5), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2, param3, param4, param5), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5, K6>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, V value, TimeSpan expiration) =>
        SaveInternal((param1, param2, param3, param4, param5, param6), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5, K6>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2, param3, param4, param5, param6), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5, K6, K7>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, K7 param7, V value, TimeSpan expiration) =>
        SaveInternal((param1, param2, param3, param4, param5, param6, param7), value, expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static V Save<K1, K2, K3, K4, K5, K6, K7>(K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, K6 param6, K7 param7, V value, TimeSpan expiration, uint limit) =>
        SaveInternal((param1, param2, param3, param4, param5, param6, param7), value, expiration, limit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static V SaveInternal<K>(K key, V value, TimeSpan expiration) where K : notnull
    {
        return new Cached<K, V>(key, default!, found: false).Save(value, expiration);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static V SaveInternal<K>(K key, V value, TimeSpan expiration, uint limit) where K : notnull
    {
        return new Cached<K, V>(key, default!, found: false).Save(value, expiration, limit);
    }
}