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
            Random.Shared.Next(int.MinValue, int.MaxValue),
            Random.Shared.Next(int.MinValue, int.MaxValue)) { }
    }

    private sealed class Uri2 : Uri
    {
        public Uri2() : base($"https://example.org{RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue)}") { }
    }

    public static void Run()
    {
        // ThreadPool.QueueUserWorkItem(_ => SeedExpirable<User>(4));
        // ThreadPool.QueueUserWorkItem(_ => SeedIndefinite<User>(15));
        ThreadPool.QueueUserWorkItem(_ => SeedExpirable<Struct>(10));
        // ThreadPool.QueueUserWorkItem(_ => Seed<Uri2>());
        // ThreadPool.QueueUserWorkItem(_ => Seed<decimal>());
        // ThreadPool.QueueUserWorkItem(_ => SeedExpirable<nuint>(75));
        // ThreadPool.QueueUserWorkItem(_ => SeedIndefinite<nuint>(125));

        Console.ReadLine();
    }

    private static void SeedExpirable<T>(int millions) where T : notnull, new()
    {
        const int secondsMin = 1;
        const int secondsMax = 180_000;
        const uint count = 250_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, millions * 4, static num =>
        {
            var sw = Stopwatch.StartNew();
            for (uint i = 0; i < count; i++)
            {
                var rand = TimeSpan.FromMilliseconds(Random.Shared.Next(secondsMin, secondsMax));

                new T().Cache(i, num, rand);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count / 10;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }

    private static void SeedIndefinite<T>(int millions) where T : notnull, new()
    {
        const uint count = 1_000_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, millions, static num =>
        {
            var expiration = TimeSpan.FromHours(1);
            var sw = Stopwatch.StartNew();
            for (uint i = 0; i < count; i++)
            {
                new T().Cache(i, num, expiration);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count / 10;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }
}
