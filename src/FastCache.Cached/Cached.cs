using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public partial struct Cached<T> where T : notnull
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
        s_cachedStore[_identifier] = new(value, expiration);
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
    public readonly TInner Value;
    public readonly long ExpiresAt;

    public CachedInner(TInner value, TimeSpan expiration)
    {
        Value = value;
        ExpiresAt = DateTime.UtcNow.Ticks + expiration.Ticks;
    }

    public CachedInner(TInner value)
    {
        Value = value;
        ExpiresAt = DateTime.MaxValue.Ticks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => DateTime.UtcNow.Ticks < ExpiresAt;
}
