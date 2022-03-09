using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();

    internal static CachedInner<T> s_default = default!;

    private readonly int _identifier;

    public readonly T Value;

    public Cached() => throw new InvalidOperationException("Cached<T> must not be initialized with default constructor");

    internal Cached(int identifier, T value)
    {
        _identifier = identifier;
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Save(T value)
    {
        Cached<T>.s_cachedStore[_identifier] = new(value);

        return value;
    }
}

public static partial class Cached
{
    // public static T? Value<T>() where T : ICachedValue
    // {
    //     return DateTime.UtcNow - Cached<T>.s_default.Timestamp > T.ExpiryPeriod()
    //         ? Cached<T>.s_default.Value
    //         : default;
    // }

    public static T? Value<T>(TimeSpan expiration) where T : notnull
    {
        return DateTime.UtcNow - Cached<T>.s_default.Timestamp > expiration
            ? Cached<T>.s_default.Value
            : default;
    }

    // public static T? ValueOr<T>(T defaultValue) where T : ICachedValue
    // {
    //     return DateTime.UtcNow - Cached<T>.s_default.Timestamp > T.ExpiryPeriod()
    //         ? Cached<T>.s_default.Value
    //         : defaultValue;
    // }
}

internal readonly struct CachedInner<TInner> where TInner : notnull
{
    public readonly TInner Value;

    public readonly DateTime Timestamp;

    public CachedInner(TInner value)
    {
        Value = value;
        Timestamp = DateTime.UtcNow;
    }
}
