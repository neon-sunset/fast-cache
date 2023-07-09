namespace FastCache.Helpers;

internal static class TimeUtils
{
#if NET6_0_OR_GREATER
    public static long Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Environment.TickCount64;
    }
#else
    private static readonly DateTime Offset = DateTime.UtcNow;

    public static long Now
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (DateTime.UtcNow - Offset).Ticks / TimeSpan.TicksPerMillisecond;
    }
#endif

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
