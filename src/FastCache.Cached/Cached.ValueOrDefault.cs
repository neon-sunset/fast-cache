namespace FastCache;

public partial struct Cached<T> where T : notnull
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? ValueOrDefault()
    {
        var (isStored, inner) = s_default;
        return isStored && inner.IsNotExpired() ? inner.Value : default;
    }
}
