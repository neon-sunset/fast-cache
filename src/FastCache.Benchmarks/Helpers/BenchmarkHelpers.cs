using System.Diagnostics.CodeAnalysis;

namespace FastCache.Benchmarks;

internal static class BenchmarkHelpers
{
    [DoesNotReturn]
    public static T Unreachable<T>() => throw new InvalidOperationException();
}
