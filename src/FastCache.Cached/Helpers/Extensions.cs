namespace FastCache.Helpers;

internal static class Extensions
{
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
