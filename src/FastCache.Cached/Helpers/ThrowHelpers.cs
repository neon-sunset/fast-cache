namespace FastCache;

internal static class ThrowHelpers
{
    public static void InvalidOperation(string message) => throw new InvalidOperationException(message);
}