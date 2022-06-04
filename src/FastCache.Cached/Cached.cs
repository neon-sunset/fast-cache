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
        var (timestamp, milliseconds) = TimeUtils.GetTimestamp(expiration);

        CacheStaticHolder<K, V>.Store[_key] = new(value, timestamp);
        CacheStaticHolder<K, V>.EvictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            CacheStaticHolder<K, V>.QuickList.Add(_key, timestamp);
        }

        return value;
    }

    public V Save(V value, TimeSpan expiration, int limit)
    {
        // TODO: Queue quicklist eviction and then fill it up with new entries. Condition: it's full or (is less or equal) to 5% of all entries.
        // Still, it's important to consider what to do for small caches where quicklist size is below (e.g. 1000s of entries)
        // Suggestion: for small enough sizes (10-100) items, perform clear inline, for more - queue a work item
        // Questions: failsafe against thrashing on maximum capacity
        // Possible compromise: thrashing with moderate perf penalty is better than a complete back-off
        return CacheStaticHolder<K, V>.Store.Count < limit ? Save(value, expiration) : value;
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
