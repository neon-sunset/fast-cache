using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;
using FastCache.Services;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly struct Cached<K, V> where K : notnull
{
    private readonly K _key;
    private readonly bool _found;

    public readonly V Value;

    /// <summary>
    ///  Cached[K, V] default constructor must not be used and will always throw.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Cached() => throw new InvalidOperationException($"Cached<{typeof(K).Name}, {typeof(V).Name}> must not be initialized with default constructor");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Cached(K key, V value, bool found)
    {
        _key = key;
        _found = found;

        Value = value;
    }

    public V Save(V value, TimeSpan expiration)
    {
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        CacheStaticHolder<K, V>.Store[_key] = new(value, timestamp);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            CacheStaticHolder<K, V>.QuickList.Add(_key, timestamp);
        }

        return value;
    }

    public V Save(V value, TimeSpan expiration, uint limit)
    {
        if (CacheStaticHolder<K, V>.Store.Count < limit || CacheManager.Trim<K, V>(Constants.FullCapacityTrimPercentage))
        {
            return Save(value, expiration);
        }

        return value;
    }

    public void Remove()
    {
        CacheStaticHolder<K, V>.Store.TryRemove(_key, out _);
    }

    [DoesNotReturn]
    private static void InvalidExpiration(TimeSpan expiration)
    {
        throw new ArgumentOutOfRangeException(nameof(expiration), expiration, "Expiration must not be negative, zero or exceed multiple years.");
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
