using System.Runtime.CompilerServices;

namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last() => Cached<T>.s_default.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1>(T1 value1)
    {
        var hashCode = HashCode.Combine(value1);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2>(T1 value1, T2 value2)
    {
        var hashCode = HashCode.Combine(value1, value2);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        var hashCode = HashCode.Combine(value1, value2, value3);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6, T7>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6, value7);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var hashCode = HashCode.Combine(value1, value2, value3, value4, value5, value6, value7, value8);

        return LastInternal(hashCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last(params object[] @params)
    {
        var hashCode = new HashCode();
        foreach (var param in @params)
        {
            hashCode.Add(param);
        }

        return LastInternal(hashCode.ToHashCode());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T? LastInternal(int identifier)
    {
        _ = Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner);

        return inner.Value;
    }
}
