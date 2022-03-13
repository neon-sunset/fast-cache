namespace FastCache;

internal static class Utils
{
    public static void InvalidOperation(string message) => throw new InvalidOperationException(message);
}