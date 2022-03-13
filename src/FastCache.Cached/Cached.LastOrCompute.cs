namespace FastCache;

public static partial class Cached
{
    public static T LastOrCompute<T, T1>(T1 param1, Func<T1, T> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, out var cached) ? cached.Value : cached.SaveIndefinitely(func(param1));

    public async static ValueTask<T> LastOrCompute<T, T1>(T1 param1, Func<T1, Task<T>> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, out var cached) ? cached.Value : cached.SaveIndefinitely(await func(param1));

    public async static ValueTask<T> LastOrCompute<T, T1>(T1 param1, Func<T1, ValueTask<T>> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, out var cached) ? cached.Value : cached.SaveIndefinitely(await func(param1));

    public static T LastOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, T> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, param2, out var cached) ? cached.Value : cached.SaveIndefinitely(func(param1, param2));

    public async static ValueTask<T> LastOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, Task<T>> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, param2, out var cached) ? cached.Value : cached.SaveIndefinitely(await func(param1, param2));

    public async static ValueTask<T> LastOrCompute<T, T1, T2>(T1 param1, T2 param2, Func<T1, T2, ValueTask<T>> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, param2, out var cached) ? cached.Value : cached.SaveIndefinitely(await func(param1, param2));

    public static T LastOrCompute<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, T> func) where T : notnull =>
        Cached<T>.TryGetLast(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.SaveIndefinitely(func(param1, param2, param3));

    public async static ValueTask<T> LastOrCompute<T, T1, T2, T3>(
        T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, Task<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3));

    public async static ValueTask<T> LastOrCompute<T, T1, T2, T3>(
        T1 param1, T2 param2, T3 param3, Func<T1, T2, T3, ValueTask<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3));

    public static T LastOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, T> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(func(param1, param2, param3, param4));

    public async static ValueTask<T> LastOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, Task<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3, param4));

    public async static ValueTask<T> LastOrCompute<T, T1, T2, T3, T4>(
        T1 param1, T2 param2, T3 param3, T4 param4, Func<T1, T2, T3, T4, ValueTask<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3, param4));

    public static T LastOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, T> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, param5, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(func(param1, param2, param3, param4, param5));

    public static async ValueTask<T> LastOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, Task<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, param5, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3, param4, param5));

    public static async ValueTask<T> LastOrCompute<T, T1, T2, T3, T4, T5>(
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, Func<T1, T2, T3, T4, T5, ValueTask<T>> func) where T : notnull =>
            Cached<T>.TryGetLast(param1, param2, param3, param4, param5, out var cached)
                ? cached.Value
                : cached.SaveIndefinitely(await func(param1, param2, param3, param4, param5));
}
