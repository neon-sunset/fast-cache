using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace FastCache;

[StructLayout(LayoutKind.Auto)]
public partial struct Cached<T> where T : notnull
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();
    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);
    internal static ExpectedExpiryMark<T> s_expiryMark = new(false, default);

    private readonly int _identifier;
    private readonly bool _saveExpiryMark;
    public readonly T Value;

    public Cached() => throw new InvalidOperationException("Cached<T> must not be initialized with default constructor");

    internal Cached(int identifier, T value, bool saveExpiryMark = false)
    {
        _identifier = identifier;
        _saveExpiryMark = saveExpiryMark;

        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Save(T value)
    {
        s_cachedStore[_identifier] = new(value);

        if (_saveExpiryMark)
        {
            // TODO: refactor API and move storing expiry timeframe and handling to 'cached.Save(value, ?timespan?)'
        }

        return value;
    }
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

internal readonly struct ExpectedExpiryMark<T> where T : notnull
{
    public readonly bool IsSet;
    public readonly TimeSpan Value;

    public ExpectedExpiryMark(bool isSet, TimeSpan value)
    {
        IsSet = isSet;
        Value = value;
    }
}
