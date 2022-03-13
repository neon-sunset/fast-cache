namespace FastCache;

public static partial class CachedExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T>(this T value, TimeSpan expiration) where T : notnull
    {
        Cached<T>.s_default = (true, new(value, expiration));
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1>(this T value, T1 value1, TimeSpan expiration) where T : notnull =>
        CacheInternal(value, HashCode.Combine(value1), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2>(this T value, T1 value1, T2 value2, TimeSpan expiration) where T : notnull =>
        CacheInternal(value, HashCode.Combine(value1, value2), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3>(this T value, T1 value1, T2 value2, T3 value3, TimeSpan expiration) where T : notnull =>
        CacheInternal(value, HashCode.Combine(value1, value2, value3), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4>(this T value, T1 value1, T2 value2, T3 value3, T4 value4, TimeSpan expiration)
        where T : notnull =>
            CacheInternal(value, HashCode.Combine(value1, value2, value3, value4), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, TimeSpan expiration) where T : notnull =>
            CacheInternal(value, HashCode.Combine(value1, value2, value3, value4, value5), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, TimeSpan expiration)
        where T : notnull =>
            CacheInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, TimeSpan expiration)
        where T : notnull =>
            CacheInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6, value7), expiration);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7, T8>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, TimeSpan expiration)
        where T : notnull =>
            CacheInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6, value7, value8), expiration);

    public static T Cache<T>(this T value, TimeSpan expiration, params object[] @params) where T : notnull
    {
        var accumulator = new HashCode();
        foreach (var param in @params)
        {
            accumulator.Add(param);
        }

        return CacheInternal(value, accumulator.ToHashCode(), expiration);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T CacheInternal<T>(T value, int identifier, TimeSpan expiration) where T : notnull
    {
        Cached<T>.s_cachedStore[identifier] = new(value, expiration);
        return value;
    }
}
