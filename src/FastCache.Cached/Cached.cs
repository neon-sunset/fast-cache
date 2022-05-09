using System.Diagnostics.CodeAnalysis;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly partial struct Cached<T> where T : notnull
{
    private readonly int _identifier;

    public readonly T Value;

    public Cached() => throw new InvalidOperationException("Cached<T> must not be initialized with default constructor");

    internal Cached(int identifier, T value)
    {
        _identifier = identifier;
        Value = value;
    }

    public T Save(T value, TimeSpan expiration)
    {
        var now = Environment.TickCount64;
        var milliseconds = (long)expiration.TotalMilliseconds;
        var expiresAt = milliseconds + now;
        if (expiresAt is <= 0)
        {
            InvalidExpiration(expiration);
        }

        s_store[_identifier] = new(value, expiresAt);
        s_quickEvictList.Add(_identifier, expiresAt);
        s_evictionJob.ReportExpiration(milliseconds);
        return value;
    }

    [DoesNotReturn]
    private static void InvalidExpiration(TimeSpan expiration)
    {
        throw new ArgumentOutOfRangeException(nameof(expiration), expiration, "Expiration must not be negative, zero or exceed many years.");
    }
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct CachedInner<T> where T : notnull
{
    private readonly long _expiresAt;

    public readonly T Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(T value, long expiresAt)
    {
        Value = value;
        _expiresAt = expiresAt;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => Environment.TickCount64 < _expiresAt;

    public void Deconstruct(out T value, out long expiresAt)
    {
        value = Value;
        expiresAt = _expiresAt;
    }
}
