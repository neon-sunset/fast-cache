namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet(TimeSpan expiration, out T value)
    {
        if (s_default.IsStored && s_default.Inner.NewerThan(expiration))
        {
            value = s_default.Inner.Value;
            return true;
        }

        value = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1>(T1 param1, TimeSpan expiration, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2>(T1 param1, T2 param2, TimeSpan expiration, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1, param2), expiration, out cached);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3>(T1 param1, T2 param2, T3 param3, TimeSpan expiration, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(param1, param2, param3), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, TimeSpan expiration, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, TimeSpan expiration, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, TimeSpan expiration, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, TimeSpan expiration, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7), expiration, out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, TimeSpan expiration, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7, param8), expiration, out cached);

    public static bool TryGet(TimeSpan expiration, out Cached<T> cached, params object[] @params)
    {
        var accumulator = new HashCode();
        foreach (var param in @params)
        {
            accumulator.Add(param);
        }

        return TryGetInternal(accumulator.ToHashCode(), expiration, out cached);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal(int identifier, TimeSpan expiration, out Cached<T> cached)
    {
        if (s_cachedStore.TryGetValue(identifier, out var inner) && inner.NewerThan(expiration))
        {
            cached = new(identifier, inner.Value, saveExpiryMark: true);
            return true;
        }

        cached = new(identifier, default!, saveExpiryMark: true);
        return false;
    }
}
