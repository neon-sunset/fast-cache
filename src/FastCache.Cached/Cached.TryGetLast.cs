namespace FastCache;

public readonly partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast(out T value)
    {
        if (s_default.IsStored)
        {
            value = s_default.Inner.Value;
            return true;
        }

        value = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1>(T1 param1, out Cached<T> holder) =>
        TryGetLastInternal(HashCode.Combine(param1), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2>(T1 param1, T2 param2, out Cached<T> holder) =>
        TryGetLastInternal(HashCode.Combine(param1, param2), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3>(T1 param1, T2 param2, T3 param3, out Cached<T> holder) =>
        TryGetLastInternal(HashCode.Combine(param1, param2, param3), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, out Cached<T> holder) =>
            TryGetLastInternal(HashCode.Combine(param1, param2, param3, param4), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, out Cached<T> holder) =>
            TryGetLastInternal(HashCode.Combine(param1, param2, param3, param4, param5), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, out Cached<T> holder) =>
            TryGetLastInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, out Cached<T> holder) =>
            TryGetLastInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetLast<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, out Cached<T> holder) =>
            TryGetLastInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7, param8), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetLastInternal(int identifier, out Cached<T> cached)
    {
        if (s_cachedStore.TryGetValue(identifier, out var inner))
        {
            cached = new(identifier, inner.Value);
            return true;
        }

        cached = new(identifier, default!);
        return false;
    }
}
