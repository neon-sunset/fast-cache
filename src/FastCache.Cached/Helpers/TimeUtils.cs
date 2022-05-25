namespace FastCache.Helpers;

internal static class TimeUtils
{
#if NETCOREAPP3_0_OR_GREATER
    public static long Now => Environment.TickCount64;
#else
    private static readonly DateTime Offset = DateTime.UtcNow;
    public static long Now => (DateTime.UtcNow - Offset).Ticks / TimeSpan.TicksPerMillisecond;
#endif
}
