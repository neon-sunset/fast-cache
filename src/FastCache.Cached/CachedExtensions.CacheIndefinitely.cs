namespace FastCache;

public static partial class CachedExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T>(this T value) where T : notnull
    {
        Cached<T>.s_default = (true, new(value));
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1>(this T value, T1 value1) where T : notnull =>
        CacheIndefinitelyInternal(value, HashCode.Combine(value1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2>(this T value, T1 value1, T2 value2) where T : notnull =>
        CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3>(this T value, T1 value1, T2 value2, T3 value3) where T : notnull =>
        CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3, T4>(this T value, T1 value1, T2 value2, T3 value3, T4 value4)
        where T : notnull =>
            CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3, value4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3, T4, T5>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) where T : notnull =>
            CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3, value4, value5));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3, T4, T5, T6>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        where T : notnull =>
            CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3, T4, T5, T6, T7>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        where T : notnull =>
            CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6, value7));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CacheIndefinitely<T, T1, T2, T3, T4, T5, T6, T7, T8>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        where T : notnull =>
            CacheIndefinitelyInternal(value, HashCode.Combine(value1, value2, value3, value4, value5, value6, value7, value8));

    public static T CacheIndefinitely<T>(this T value, params object[] @params) where T : notnull
    {
        var accumulator = new HashCode();
        foreach (var param in @params)
        {
            accumulator.Add(param);
        }

        Cached<T>.s_cachedStore[accumulator.ToHashCode()] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T CacheIndefinitelyInternal<T>(T value, int identifier) where T : notnull
    {
        Cached<T>.s_cachedStore[identifier] = new(value);
        return value;
    }
}
