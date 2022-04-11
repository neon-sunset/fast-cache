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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Save(T value, TimeSpan expiration)
    {
        var expiresAtTicks = (DateTime.UtcNow + expiration).Ticks;
        s_cachedStore[_identifier] = new(value, expiresAtTicks);
        s_oldestEntries.Add(_identifier, expiresAtTicks);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SaveIndefinitely(T value)
    {
        s_cachedStore[_identifier] = new(value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SaveLRU(T value)
    {
        throw new NotImplementedException();
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
