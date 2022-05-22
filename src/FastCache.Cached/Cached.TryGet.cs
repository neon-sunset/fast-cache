using FastCache.Helpers;

namespace FastCache;

public readonly partial struct Cached<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1>(T1 param1, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(Dispersed(param1)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2>(T1 param1, T2 param2, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3>(T1 param1, T2 param2, T3 param3, out Cached<T> cached) =>
        TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3), Dispersed(param4)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3), Dispersed(param4), Dispersed(param5)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3), Dispersed(param4), Dispersed(param5), Dispersed(param6)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3), Dispersed(param4), Dispersed(param5), Dispersed(param6), Dispersed(param7)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, out Cached<T> cached) =>
            TryGetInternal(HashCode.Combine(Dispersed(param1), Dispersed(param2), Dispersed(param3), Dispersed(param4), Dispersed(param5), Dispersed(param6), Dispersed(param7), Dispersed(param8)), out cached);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal(int identifier, out Cached<T> cached)
    {
        var found = s_store.TryGetValue(identifier, out var inner);
        if (found && inner.IsNotExpired())
        {
            cached = new(identifier, found: true, inner.Value);
            return true;
        }

        cached = new(identifier, found: false, default!);
        return false;
    }

    private static int Dispersed<TParam>(TParam value) => HashCode.Combine(value, TypeHash<TParam>.Value);
}
