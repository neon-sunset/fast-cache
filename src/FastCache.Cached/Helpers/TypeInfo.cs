namespace FastCache.Helpers;

internal static class TypeInfo
{
    // Will miss nested reference types on netstandard2.0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsManaged<T>()
    {
#if NETSTANDARD2_0
        return !typeof(T).IsValueType;
#else
        return RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
    }
}
