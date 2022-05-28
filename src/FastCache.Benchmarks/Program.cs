using BenchmarkDotNet.Running;
using FastCache.Benchmarks;

namespace Namespace
{
    public static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Comparison>();
        }
    }
}
