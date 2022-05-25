using System.Diagnostics.CodeAnalysis;
using FastCache.Helpers;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly partial struct Cached<T>
{
    private readonly int _identifier;
    private readonly bool _found;

    public readonly T Value;

    public Cached() => throw new InvalidOperationException($"Cached<{typeof(T).Name}> must not be initialized with default constructor");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Cached(int identifier, bool found, T value)
    {
        _identifier = identifier;
        _found = found;

        Value = value;
    }

    public T Save(T value, TimeSpan expiration)
    {
        var now = TimeUtils.Now;
        var milliseconds = expiration.Ticks / TimeSpan.TicksPerMillisecond;

        var expiresAt = now + milliseconds;
        if (expiresAt < now)
        {
            InvalidExpiration(expiration);
        }

        s_store[_identifier] = new(value, expiresAt);
        s_evictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            s_quickList.Add(_identifier, expiresAt);
        }

        return value;
    }

    public void Remove()
    {
        s_store.TryRemove(_identifier, out _);
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

    public void Deconstruct(out T value, out long expiresAt)
    {
        value = Value;
        expiresAt = _expiresAt;
    }
}
