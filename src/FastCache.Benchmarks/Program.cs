using BenchmarkDotNet.Running;
using FastCache.Benchmarks;

namespace Namespace
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Comparison>();
        }
    }
}