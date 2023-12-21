using System.Runtime.CompilerServices;

namespace FastCache.Tests;

internal static class TestHelpers
{
    public static string GetTestKey([CallerMemberName] string testName = "")
    {
        return testName;
    }

    public static string GetTestKey<T>(T arg1, [CallerMemberName] string testName = "")
    {
        return $"{testName}:{arg1}";
    }

    public static string GetTestKey<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3, [CallerMemberName] string testName = "")
    {
        return testName + string.Join(":", arg1, arg2, arg3);
    }

    public static string GetRandomString()
    {
        var bytes = (stackalloc byte[64]);
        Random.Shared.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}
