using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly struct Cached<K, V> where K : notnull
{
    private readonly K _key;
    private readonly bool _found;

    public readonly V Value;

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
        var now = TimeUtils.Now;
        var milliseconds = expiration.Ticks / TimeSpan.TicksPerMillisecond;

        var expiresAt = now + milliseconds;
        if (expiresAt < now)
        {
            InvalidExpiration(expiration);
        }

        CacheStaticHolder<K, V>.s_store[_key] = new(value, expiresAt);
        CacheStaticHolder<K, V>.s_evictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            CacheStaticHolder<K, V>.s_quickList.Add(_key, expiresAt);
        }

        return value;
    }

    public V Save(V value, TimeSpan expiration, int limit)
    {
        return CacheStaticHolder<K, V>.s_store.Count < limit ? Save(value, expiration) : value;
    }

    public void Remove()
    {
        CacheStaticHolder<K, V>.s_store.TryRemove(_key, out _);
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
    internal readonly long _expiresAt;

    public readonly T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(T value, long expiresAt)
    {
        Value = value;
        _expiresAt = expiresAt;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => TimeUtils.Now < _expiresAt;
}
