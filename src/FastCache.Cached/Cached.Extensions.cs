using FastCache.Helpers;

namespace FastCache.Extensions;

public static class CachedExtensions
{
    public static T Cache<T>(this T value, TimeSpan expiration)
    {
        var expiresAt = Environment.TickCount64 + (long)expiration.TotalMilliseconds;

        Cached<T>.s_default = (true, new(value, expiresAt));
        return value;
    }

    public static T Cache<T, T1>(this T value, T1 value1, TimeSpan expiration) =>
        CacheInternal(value, HashCode.Combine(Dispersed(value1)), expiration);

    public static T Cache<T, T1, T2>(this T value, T1 value1, T2 value2, TimeSpan expiration) =>
        CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2)), expiration);

    public static T Cache<T, T1, T2, T3>(this T value, T1 value1, T2 value2, T3 value3, TimeSpan expiration) =>
        CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3)), expiration);

    public static T Cache<T, T1, T2, T3, T4>(this T value, T1 value1, T2 value2, T3 value3, T4 value4, TimeSpan expiration)
        =>
            CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3), Dispersed(value4)), expiration);

    public static T Cache<T, T1, T2, T3, T4, T5>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, TimeSpan expiration) =>
            CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3), Dispersed(value4), Dispersed(value5)), expiration);

    public static T Cache<T, T1, T2, T3, T4, T5, T6>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, TimeSpan expiration)
        =>
            CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3), Dispersed(value4), Dispersed(value5), Dispersed(value6)), expiration);

    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, TimeSpan expiration)
        =>
            CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3), Dispersed(value4), Dispersed(value5), Dispersed(value6), Dispersed(value7)), expiration);

    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7, T8>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, TimeSpan expiration)
        =>
            CacheInternal(value, HashCode.Combine(Dispersed(value1), Dispersed(value2), Dispersed(value3), Dispersed(value4), Dispersed(value5), Dispersed(value6), Dispersed(value7), Dispersed(value8)), expiration);

    private static T CacheInternal<T>(T value, int identifier, TimeSpan expiration)
    {
        return new Cached<T>(identifier, found: false, default!).Save(value, expiration);
    }

    private static int Dispersed<T>(T value) => HashCode.Combine(value, TypeHash<T>.Value);
}
