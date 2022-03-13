namespace FastCache.Extensions;

public static class CachedExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool NewerThan<T>(this CachedInner<T> inner, TimeSpan expiration) where T : notnull
    {
        return DateTime.UtcNow - inner.Timestamp < expiration;
    }
}
