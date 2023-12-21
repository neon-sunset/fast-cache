namespace FastCache.Helpers;

internal static class TypeInfo
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsManaged<T>() => RuntimeHelpers.IsReferenceOrContainsReferences<T>();
}
