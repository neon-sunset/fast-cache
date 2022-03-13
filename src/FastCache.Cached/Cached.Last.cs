namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last() => s_default.IsStored ? s_default.Inner.Value : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1>(T1 value1) => LastInternal(HashCode.Combine(value1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2>(T1 value1, T2 value2) => LastInternal(HashCode.Combine(value1, value2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3>(T1 value1, T2 value2, T3 value3) => LastInternal(HashCode.Combine(value1, value2, value3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) =>
        LastInternal(HashCode.Combine(value1, value2, value3, value4));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) =>
            LastInternal(HashCode.Combine(value1, value2, value3, value4, value5));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6) =>
            LastInternal(HashCode.Combine(value1, value2, value3, value4, value5, value6));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6, T7>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7) =>
            LastInternal(HashCode.Combine(value1, value2, value3, value4, value5, value6, value7));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8) =>
            LastInternal(HashCode.Combine(value1, value2, value3, value4, value5, value6, value7, value8));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Last(params object[] @params)
    {
        var accumulator = new HashCode();
        foreach (var param in @params)
        {
            accumulator.Add(param);
        }

        return LastInternal(accumulator.ToHashCode());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T? LastInternal(int identifier)
    {
        return s_cachedStore.TryGetValue(identifier, out var inner) ? inner.Value : default;
    }
}
