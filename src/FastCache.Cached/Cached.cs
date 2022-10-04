using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;
using FastCache.Services;

namespace FastCache;

/// <summary>
/// Cache entry holder. Use 'Value' after 'TryGet()' that returned true.
/// </summary>
/// <typeparam name="K">Entry key. This can be a composite key expressed as (K1..K7).</typeparam>
/// <typeparam name="V">Entry value. It is available if 'TryGet' has returned true. Accessing it otherwise is likely to return 'default'.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly record struct Cached<K, V> where K : notnull
{
    private readonly bool _found;

    /// <summary>
    /// Cache entry key. Either a single-argument like string, int, etc. or multi-key value as tuple like (int, int, bool).
    /// </summary>
    public readonly K Key;

    /// <summary>
    /// Cache entry value. Guaranteed to be up-to-date with millisecond accuracy as long as 'bool TryGet' conditional is checked.
    /// </summary>
    public readonly V Value;

    /// <summary>
    ///  Cached[K, V] default constructor must not be used and will always throw.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Cached() => throw new InvalidOperationException($"Cached<{typeof(K).Name}, {typeof(V).Name}> must not be initialized with default constructor");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Cached(K key, V value, bool found)
    {
        _found = found;

        Key = key;
        Value = value;
    }

    /// <summary>
    /// Saves the value to cache. The value will expire once the provided interval of time passes.
    /// </summary>
    /// <param name="value">Value to save</param>
    /// <param name="expiration">Interval in which the value will expire</param>
    /// <returns>Saved value</returns>
    public V Save(V value, TimeSpan expiration)
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        CacheStaticHolder<K, V>.Store[Key] = new(value, timestamp);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            CacheStaticHolder<K, V>.QuickList.Add(Key, timestamp);
        }

        return value;
    }

    /// <summary>
    /// Saves the value to cache. The value will expire once the provided interval of time passes.
    /// If the cache entries count is equal or above the limit, the cache is either trimmed to make place
    /// or trimming is queued on a threadpool and the value is returned as is.
    /// </summary>
    /// <param name="value">Value to save</param>
    /// <param name="expiration">Interval in which the value will expire</param>
    /// <param name="limit">Limit for cache entries count</param>
    /// <returns>Saved value</returns>
    public V Save(V value, TimeSpan expiration, uint limit)
    {
        if (CacheStaticHolder<K, V>.Store.Count < limit || CacheManager.Trim<K, V>(Constants.FullCapacityTrimPercentage))
        {
            return Save(value, expiration);
        }

        return value;
    }

    /// <summary>
    /// Updates cached value without updating its expiration.
    /// </summary>
    /// <param name="value">Updated value</param>
    /// <returns>True if the value has been updated successfully. False if the value is no longer present in cache.</returns>
    public bool Update(V value)
    {
        var store = CacheStaticHolder<K, V>.Store;

        if (_found && store.TryGetValue(Key, out var inner))
        {
            store[Key] = new(value, inner._timestamp);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the value from cache if it's present.
    /// </summary>
    public void Remove()
    {
        CacheStaticHolder<K, V>.Store.TryRemove(Key, out _);
    }
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct CachedInner<T>
{
    internal readonly long _timestamp;

    public readonly T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(T value, long timestamp)
    {
        Value = value;
        _timestamp = timestamp;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => TimeUtils.Now < _timestamp;
}
