using System.Diagnostics.CodeAnalysis;

namespace FastCache.Helpers;

internal static class ThrowHelpers
{
    [DoesNotReturn]
    public static void ArgumentOutOfRange<T>(T argument, string argumentName)
    {
        throw new ArgumentOutOfRangeException(argumentName, $"Actual value: {argument}");
    }

    [DoesNotReturn]
    public static void InvalidExpiration(TimeSpan expiration)
    {
        throw new ArgumentOutOfRangeException(nameof(expiration), expiration, "Expiration must not be negative or zero.");
    }

    [DoesNotReturn]
    public static void IncorrectSaveLength(int keys, int values)
    {
        throw new ArgumentOutOfRangeException(nameof(values), values, $"Cannot perform 'Save()' for provided ranges - length mismatch. Expected: {keys}.");
    }
}
