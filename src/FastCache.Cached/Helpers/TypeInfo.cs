namespace FastCache.Helpers;

internal static class TypeInfo
{
    // Will miss nested reference types on netstandard2.0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NETSTANDARD2_0
    public static bool IsManaged<T>() => !typeof(T).IsValueType;
#else
    public static bool IsManaged<T>() => RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
}
