namespace FastCache.Helpers;

internal static class TypeHash<T>
{
    public static readonly int Value = typeof(T).GetHashCode();
}
