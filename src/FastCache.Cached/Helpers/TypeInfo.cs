namespace FastCache.Helpers;

internal static class TypeInfo<T>
{
    // Will miss nested reference types on netstandard2.0
#if NETSTANDARD2_0
    public static readonly bool IsManaged = !typeof(T).IsValueType;
#else
    public static readonly bool IsManaged = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
}
