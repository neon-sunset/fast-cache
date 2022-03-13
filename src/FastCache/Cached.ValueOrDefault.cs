namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ValueOrDefault(TimeSpan expiration)
    {
        var (isStored, inner) = s_default;
        return isStored && inner.NewerThan(expiration) ? inner.Value : default;
    }
}
