using System.Diagnostics.CodeAnalysis;

namespace FastCache.Helpers;

internal static class ThrowHelpers
{
    [DoesNotReturn]
    public static void ArgumentOutOfRange<T>(T argument, string argumentName)
    {
        throw new ArgumentOutOfRangeException(argumentName, $"Actual value: {argument}");
    }
}
