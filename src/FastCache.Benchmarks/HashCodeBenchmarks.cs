using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using FastCache.Helpers;

namespace FastCache.Benchmarks;

public class HashCodeBenchmarks
{
    private record User(string Login, string Password, string Name, string Email, DateTime DateOfBirth, decimal Balance)
    {
        public User() : this(
            $"Login {Random.Shared.NextInt64()}",
            $"Password {Random.Shared.NextInt64()}",
            $"Name {Random.Shared.NextInt64()}",
            $"Email {Random.Shared.NextInt64()}",
            DateTime.Now,
            (decimal)Random.Shared.NextDouble()) { }
    }

    private readonly record struct Struct(int Key, int Value)
    {
        public Struct() : this(
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue),
            RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue)) { }
    }

    private sealed class Uri2 : Uri
    {
        public Uri2() : base($"https://example.org{RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue)}") { }
    }

    [Benchmark(Baseline = true)]
    public int TypeHashCode() => Compute<User>();

    [Benchmark]
    public int TypeMetadataToken() => StaticTypeHashCode<User>();

    // [Benchmark]
    // public int TypeOfClassXxHash() => ComputeXxHash<User>();

    // [Benchmark]
    // public int TypeOfClassName() => ComputeTypeName<User>();

    private static int Compute<T>() => typeof(T).GetHashCode();

    private static int ComputeXxHash<T>() => HashCode.Combine(typeof(T));

    private static int ComputeTypeName<T>() => typeof(T).Name.GetHashCode();

    private static int StaticTypeHashCode<T>() => TypeHash<T>.Value;
}
