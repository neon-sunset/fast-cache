using System.Diagnostics.CodeAnalysis;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly partial struct Cached<T> where T : notnull
{
    private readonly int _identifier;
    private readonly bool _found;

    public readonly T Value;

    public Cached() => throw new InvalidOperationException("Cached<T> must not be initialized with default constructor");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Cached(int identifier, bool found, T value)
    {
        _identifier = identifier;
        _found = found;

        Value = value;
    }

    public T Save(T value, TimeSpan expiration)
    {
        var now = Environment.TickCount64;
        var milliseconds = (long)expiration.TotalMilliseconds;
        var expiresAt = milliseconds + now;
        if (expiresAt <= 0)
        {
            InvalidExpiration(expiration);
        }

        s_store[_identifier] = new(value, expiresAt);
        s_evictionJob.ReportExpiration(milliseconds);

        if (!_found)
        {
            s_quickEvictList.Add(_identifier, expiresAt);
        }

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
    internal readonly long _expiresAt;

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
