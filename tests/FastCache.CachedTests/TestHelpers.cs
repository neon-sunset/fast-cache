using System.Runtime.CompilerServices;

namespace FastCache.Tests;

internal static class TestHelpers
{
    private static readonly Random _random = new();

    public static string GetTestKey([CallerMemberName] string testName = "")
    {
        return testName;
    }

    public static string GetTestKey<T>(T arg1, [CallerMemberName] string testName = "")
    {
        return $"{testName}:{arg1}";
    }

    public static string GetTestKey<T1, T2>(T1 arg1, T2 arg2, [CallerMemberName] string testName = "")
    {
        return testName + string.Join(':', arg1, arg2);
    }

    public static string GetTestKey<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, [CallerMemberName] string testName = "")
    {
        return testName + string.Join(':', arg1, arg2, arg3);
    }

    public static string GetTestKey<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, [CallerMemberName] string testName = "")
    {
        return testName + string.Join(':', arg1, arg2, arg3, arg4);
    }

    public static string GetRandomString()
    {
#if !NETSTANDARD2_0
        var bytes = (stackalloc byte[256]);
#else
        var bytes = new byte[256];
#endif

        _random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }

    public static long GetRandomLong()
    {
#if !NETSTANDARD2_0
        var bytes = (stackalloc byte[8]);
#else
        var bytes = new byte[8];
#endif

        _random.NextBytes(bytes);

#if !NETSTANDARD2_0
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
