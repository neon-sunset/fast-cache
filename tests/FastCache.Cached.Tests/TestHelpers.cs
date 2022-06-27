namespace FastCache.Tests;

internal static class TestHelpers
{
    private static readonly Random _random = new();

    public static string RandomString()
    {
#if !NET48
        var bytes = (stackalloc byte[64]);
#else
        var bytes = new byte[64];
#endif

        _random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }

    public static long RandomLong()
    {
#if !NET48
        var bytes = (stackalloc byte[8]);
#else
        var bytes = new byte[8];
#endif

        _random.NextBytes(bytes);

#if !NET48
        return BitConverter.ToInt64(bytes);
#else
        return BitConverter.ToInt64(bytes, 0);
#endif
    }

    public static void Unreachable()
    {
        throw new InvalidOperationException("This part of code should never be reached");
    }

    public static T Unreachable<T>()
    {
        throw new InvalidOperationException("This part of code should never be reached");
    }
}
