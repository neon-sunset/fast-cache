namespace FastCache;

public static class Cached
{
    public static T GetOrCompute<T, T1>(T1 param1, Func<T1, T> func, TimeSpan expiration) =>
        Cached<T>.TryGet(param1, out var cached) ? cached.Value : cached.Save(func(param1), expiration);

    public static async ValueTask<T> GetOrCompute<T, T1>(T1 param1, Func<T1, Task<T>> func, TimeSpan expiration) =>
        Cached<T>.TryGet(param1, out var cached) ? cached.Value : cached.Save(await func(param1), expiration);

    public static async ValueTask<T> GetOrCompute<T, T1>(T1 param1, Func<T1, ValueTask<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, out var cached) ? cached.Value : cached.Save(await func(param1), expiration);
    }

    public static T GetOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, T> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(func(param1, param2), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, Task<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(await func(param1, param2), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, ValueTask<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(await func(param1, param2), expiration);
    }

    public static T GetOrCompute<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, T> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3>(
        T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, Task<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3>(
        T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, ValueTask<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3), expiration);
    }

    public static T GetOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, T> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3, param4), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, Task<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, ValueTask<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4), expiration);
    }

    public static T GetOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, T> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3, param4, param5), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, Task<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4, param5), expiration);
    }

    public static async ValueTask<T> GetOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, ValueTask<T>> func, TimeSpan expiration)
    {
        return Cached<T>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4, param5), expiration);
    }
}
