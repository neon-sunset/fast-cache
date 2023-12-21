namespace FastCache.Helpers;

internal static class TimeUtils
{
    public static long Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Environment.TickCount64;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (long timestamp, long offset) GetTimestamp(TimeSpan expiration)
    {
        var now = Now;
        var milliseconds = (long)Math.Ceiling(expiration.Ticks / (double)TimeSpan.TicksPerMillisecond);

        var timestamp = now + milliseconds;
        if (timestamp <= now)
        {
            ThrowHelpers.InvalidExpiration(expiration);
        }

        return (timestamp, milliseconds);
    }
}
