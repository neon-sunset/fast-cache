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
        var expiresAtTicks = GetExpirationTicks(expiration);

        s_cachedStore[_identifier] = new(value, expiresAtTicks);
        s_quickEvictList.Add(_identifier, expiresAtTicks);
        return value;
    }

    public T SaveIndefinitely(T value)
    {
        s_cachedStore[_identifier] = new(value);
        return value;
    }

    private static int GetExpirationTicks(TimeSpan expiration) => Environment.TickCount + (int)expiration.TotalMilliseconds;
}

[StructLayout(LayoutKind.Auto)]
internal readonly struct CachedInner<TInner> where TInner : notnull
{
    private const int MaxExpirationTicks = int.MaxValue;

    public readonly TInner Value;
    private readonly int _expiresAt;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(TInner value, int expiresAt)
    {
        Value = value;
        _expiresAt = expiresAt;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CachedInner(TInner value)
    {
        Value = value;
        _expiresAt = MaxExpirationTicks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotExpired() => Environment.TickCount < _expiresAt;

    public void Deconstruct(out TInner value, out int expiresAt)
    {
        value = Value;
        expiresAt = _expiresAt;
    }
}
