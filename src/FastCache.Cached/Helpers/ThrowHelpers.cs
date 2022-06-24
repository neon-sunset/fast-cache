using System.Diagnostics.CodeAnalysis;

namespace FastCache.Helpers;

internal static class ThrowHelpers
{

#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.NoInlining)]
#else
    [DoesNotReturn]
#endif
    public static void ArgumentOutOfRange<T>(T argument, string argumentName)
    {
        throw new ArgumentOutOfRangeException(argumentName, $"Actual value: {argument}");
    }

#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.NoInlining)]
#else
    [DoesNotReturn]
#endif
    public static void InvalidExpiration(TimeSpan expiration)
    {
        throw new ArgumentOutOfRangeException(nameof(expiration), expiration, "Expiration must not be negative, zero or exceed multiple years.");
    }

#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.NoInlining)]
#else
    [DoesNotReturn]
#endif
    public static void IncorrectSaveLength(int keys, int values)
    {
        throw new ArgumentOutOfRangeException(nameof(values), values, $"Cannot perform 'Save()' for provided ranges - length mismatch. Expected: {keys}.");
    }
}
