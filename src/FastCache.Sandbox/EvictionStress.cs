using System.Diagnostics;
using System.Security.Cryptography;
using FastCache.Extensions;
using FastCache.Services;

namespace FastCache.Sandbox;

public static class EvictionStress
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

    public static void Run()
    {
        ThreadPool.QueueUserWorkItem(_ => Seed<User>());
        ThreadPool.QueueUserWorkItem(_ => Seed<Struct>());
        ThreadPool.QueueUserWorkItem(_ => Seed<Uri2>());
        ThreadPool.QueueUserWorkItem(_ => Seed<decimal>());
        ThreadPool.QueueUserWorkItem(_ => Seed<nuint>());

        Console.ReadLine();
    }

    private static void Seed<T>() where T : notnull, new()
    {
        const uint count = 1_000_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, 15, static num =>
        {
            var sw = Stopwatch.StartNew();
            for (uint i = 0; i < count; i++)
            {
                var rand = TimeSpan.FromMilliseconds(1);

                new T().Cache(i, num, rand);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} ticks per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }
}
