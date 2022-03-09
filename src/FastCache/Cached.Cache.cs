using System.Runtime.CompilerServices;

namespace FastCache;

public static partial class Cached
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T>(this T value) where T : notnull
    {
        Cached<T>.s_default = new(value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1>(this T value, T1 value1) where T : notnull
    {
        var hashCode = HashCode.Combine(value1);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2>(this T value, T1 value1, T2 value2) where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3>(this T value, T1 value1, T2 value2, T3 value3) where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4>(this T value, T1 value1, T2 value2, T3 value3, T4 value4)
        where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6, value7);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cache<T, T1, T2, T3, T4, T5, T6, T7, T8>(
        this T value, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        where T : notnull
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6, value7, value8);
        Cached<T>.s_cachedStore[hashCode] = new(value);

        return value;
    }

    public static T Cache<T>(this T value, params object[] @params) where T : notnull
    {
        var hashCode = new HashCode();
        foreach (var param in @params)
        {
            hashCode.Add(param);
        }

        Cached<T>.s_cachedStore[hashCode.ToHashCode()] = new(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T CacheInternal<T>(this T value, int identifier) where T : notnull
    {
        Cached<T>.s_cachedStore[identifier] = new(value);
        return value;
    }
}
