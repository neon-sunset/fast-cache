namespace FastCache;

public readonly partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet(out T value)
    {
        if (s_default.IsStored && s_default.Inner.IsNotExpired())
        {
            value = s_default.Inner.Value;
            return true;
        }

        value = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1>(T1 param1, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2>(T1 param1, T2 param2, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1, param2), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3>(T1 param1, T2 param2, T3 param3, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1, param2, param3), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7, param8), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal(int identifier, out Cached<T> cached)
    {
        bool notExpiredOrNew = true;
        if (s_store.TryGetValue(identifier, out var inner) && (notExpiredOrNew = inner.IsNotExpired()))
        {
            cached = new(identifier, updatesExisting: true, inner.Value);
            return true;
        }

        cached = new(identifier, !notExpiredOrNew, default!);
        return false;
    }
}
