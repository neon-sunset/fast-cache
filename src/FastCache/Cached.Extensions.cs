namespace FastCache;

public static partial class Cached
{
    public static T GetOrCompute<T, T1>(T1 param1, Func<T1, T> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return func(param1).CacheInternal(identifier);
    }

    public async static ValueTask<T> GetOrCompute<T, T1>(T1 param1, Func<T1, Task<T>> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return (await func(param1)).CacheInternal(identifier);
    }

    public static T GetOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, T> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return func(param1, param2).CacheInternal(identifier);
    }

    public async static ValueTask<T> GetOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, Task<T>> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return (await func(param1, param2)).CacheInternal(identifier);
    }

    public static T GetOrCompute<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, T> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2, param3);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return func(param1, param2, param3).CacheInternal(identifier);
    }

    public async static ValueTask<T> GetOrCompute<T, T1, T2, T3>(
        T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, Task<T>> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2, param3);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return (await func(param1, param2, param3)).CacheInternal(identifier);
    }

    public static T GetOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, T> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2, param3);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return func(param1, param2, param3, param4).CacheInternal(identifier);
    }

    public async static ValueTask<T> GetOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, Task<T>> func) where T : notnull
    {
        var identifier = HashCode.Combine(param1, param2, param3);
        if (Cached<T>.s_cachedStore.TryGetValue(identifier, out var inner))
        {
            return inner.Value;
        }

        return (await func(param1, param2, param3, param4)).CacheInternal(identifier);
    }

    public static T? Last<T>() where T : notnull => Cached<T>.Last();
    public static T? Last<T, T1>(T1 param1) where T : notnull => Cached<T>.Last(param1);
    public static T? Last<T, T1, T2>(T1 param1, T2 param2) where T : notnull => Cached<T>.Last(param1, param2);
    public static T? Last<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : notnull => Cached<T>.Last(param1, param2, param3);
    public static T? Last<T, T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4) where T : notnull =>
        Cached<T>.Last(param1, param2, param3, param4);
    public static T? Last<T, T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) where T : notnull =>
        Cached<T>.Last(param1, param2, param3, param4, param5);
    public static T? Last<T, T1, T2, T3, T4, T5, T6>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) where T : notnull =>
        Cached<T>.Last(param1, param2, param3, param4, param5, param6);
    public static T? Last<T, T1, T2, T3, T4, T5, T6, T7>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7) where T : notnull =>
            Cached<T>.Last(param1, param2, param3, param4, param5, param6, param7);
    public static T? Last<T, T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8) where T : notnull =>
            Cached<T>.Last(param1, param2, param3, param4, param5, param6, param7, param8);
}
