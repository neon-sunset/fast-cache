namespace FastCache;

public static class Cached
{
    public static V GetOrCompute<K, V>(K param1, Func<K, V> func, TimeSpan expiration) where K : notnull
    {
        return Cached<V>.TryGet(param1, out var cached) ? cached.Value : cached.Save(func(param1), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K, V>(K param1, Func<K, Task<V>> func, TimeSpan expiration) where K : notnull
    {
        return Cached<V>.TryGet(param1, out var cached) ? cached.Value : cached.Save(await func(param1), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K, V>(K param1, Func<K, ValueTask<V>> func, TimeSpan expiration) where K : notnull
    {
        return Cached<V>.TryGet(param1, out var cached) ? cached.Value : cached.Save(await func(param1), expiration);
    }

    public static V GetOrCompute<K1, K2, V>(K1 param1, K2 param2, Func<K1, K2, V> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(func(param1, param2), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, V>(K1 param1, K2 param2, Func<K1, K2, Task<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(await func(param1, param2), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, V>(K1 param1, K2 param2, Func<K1, K2, ValueTask<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, out var cached) ? cached.Value : cached.Save(await func(param1, param2), expiration);
    }

    public static V GetOrCompute<K1, K2, K3, V>(K1 param1, K2 param2, K3 param3, Func<K1, K2, K3, V> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, V>(
        K1 param1, K2 param2, K3 param3, Func<K1, K2, K3, Task<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, V>(
        K1 param1, K2 param2, K3 param3, Func<K1, K2, K3, ValueTask<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3), expiration);
    }

    public static V GetOrCompute<K1, K2, K3, K4, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, Func<K1, K2, K3, K4, V> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3, param4), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, K4, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, Func<K1, K2, K3, K4, Task<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, K4, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, Func<K1, K2, K3, K4, ValueTask<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4), expiration);
    }

    public static V GetOrCompute<K1, K2, K3, K4, K5, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, Func<K1, K2, K3, K4, K5, V> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(func(param1, param2, param3, param4, param5), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, K4, K5, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, Func<K1, K2, K3, K4, K5, Task<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4, param5), expiration);
    }

    public static async ValueTask<V> GetOrCompute<K1, K2, K3, K4, K5, V>(
        K1 param1, K2 param2, K3 param3, K4 param4, K5 param5, Func<K1, K2, K3, K4, K5, ValueTask<V>> func, TimeSpan expiration)
    {
        return Cached<V>.TryGet(param1, param2, param3, param4, param5, out var cached)
            ? cached.Value
            : cached.Save(await func(param1, param2, param3, param4, param5), expiration);
    }
}
