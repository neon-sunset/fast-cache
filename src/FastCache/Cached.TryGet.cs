using System.Runtime.CompilerServices;

namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet(out T value)
    {
        if (Cached<T>.s_default.Value is not null)
        {
            value = Cached<T>.s_default.Value;
            return true;
        }

        value = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1>(T1 param1, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1>(T1 param1, TimeSpan expiration, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2>(T1 param1, T2 param2, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1, param2), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2>(T1 param1, T2 param2, TimeSpan expiration, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1, param2), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3>(T1 param1, T2 param2, T3 param3, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1, param2, param3), out holder);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3>(T1 param1, T2 param2, T3 param3, TimeSpan expiration, out Cached<T> holder) =>
        TryGetInternal(HashCode.Combine(param1, param2, param3), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, TimeSpan expiration, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, TimeSpan expiration, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, TimeSpan expiration, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, TimeSpan expiration, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7), expiration, out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7, param8), out holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGet<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, TimeSpan expiration, out Cached<T> holder) =>
            TryGetInternal(HashCode.Combine(param1, param2, param3, param4, param5, param6, param7, param8), expiration, out holder);

    public static bool TryGet(TimeSpan expiration, out Cached<T> holder, params object[] @params)
    {
        var identifier = new HashCode();
        foreach (var param in @params)
        {
            identifier.Add(param);
        }

        return TryGetInternal(identifier.ToHashCode(), expiration, out holder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal(int identifier, TimeSpan expiration, out Cached<T> holder)
    {
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner)
            && DateTime.UtcNow - inner.Timestamp > expiration)
        {
            holder = new(identifier, inner.Value);
            return true;
        }

        holder = new(identifier, default!);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetInternal(int identifier, out Cached<T> holder)
    {
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            holder = new(identifier, inner.Value);
            return true;
        }

        holder = new(identifier, default!);
        return false;
    }
}
