using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public readonly partial struct Cached<T> where T : notnull
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();
    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);

    private readonly int _identifier;
    public readonly T Value;

    public Cached() => throw new InvalidOperationException("Cached<T> must not be initialized with default constructor");

    internal Cached(int identifier, T value)
    {
        _identifier = identifier;
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Save(T value, TimeSpan expiration)
    {
        var expiresAt = DateTime.UtcNow + expiration;
        s_cachedStore[_identifier] = new(value, expiresAt.Ticks);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SaveIndefinitely(T value)
    {
        s_cachedStore[_identifier] = new(value);
        return value;
    }
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct CachedInner<TInner> where TInner : notnull
{
    private static readonly long MaxExpirationTicks = DateTime.MaxValue.Ticks;

    public readonly TInner Value;
    public readonly long ExpiresAtTicks;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(TInner value, long expiresAtTicks)
    {
        Value = value;
        ExpiresAtTicks = expiresAtTicks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(TInner value)
    {
        Value = value;
        ExpiresAtTicks = MaxExpirationTicks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => DateTime.UtcNow.Ticks < ExpiresAtTicks;
}
