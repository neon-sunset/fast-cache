using System.Diagnostics.CodeAnalysis;

namespace FastCache.Benchmarks;

internal static class BenchmarkHelpers
{
#if !NET48
    [DoesNotReturn]
#endif
    public static T Unreachable<T>() => throw new InvalidOperationException();
}
