namespace FastCache.Benchmarks;

internal static class BenchmarkHelpers
{
    public static T Unreachable<T>() => throw new InvalidOperationException();
}