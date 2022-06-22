﻿using System.Diagnostics.CodeAnalysis;

namespace FastCache.Cached.Tests;

internal static class TestHelpers
{
    private static readonly Random _random = new();

    public static string RandomString()
    {
        var bytes = (stackalloc byte[64]);

        _random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }

    [DoesNotReturn]
    public static void Unreachable()
    {
        throw new InvalidOperationException("This part of code should never be reached");
    }

    [DoesNotReturn]
    public static T Unreachable<T>()
    {
        throw new InvalidOperationException("This part of code should never be reached");
    }
}
