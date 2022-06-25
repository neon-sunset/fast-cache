namespace FastCache.Helpers;

internal static class Extensions
{
#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<K, V>(this KeyValuePair<K, V> kvp, out K key, out V value) where K : notnull
    {
        key = kvp.Key;
        value = kvp.Value;
    }
#endif

    // These exist for backwards compatibility with netstandard2.0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan DivideBy(this TimeSpan value, uint divisor)
    {
        var milliseconds = value.TotalMilliseconds;
        var result = milliseconds / divisor;

        return TimeSpan.FromMilliseconds(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan MultiplyBy(this TimeSpan value, uint multiplier)
    {
        var milliseconds = value.TotalMilliseconds;
        var result = milliseconds * multiplier;

        return TimeSpan.FromMilliseconds(result);
    }
}
