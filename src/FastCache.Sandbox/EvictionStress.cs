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
        ThreadPool.QueueUserWorkItem(_ => SeedSequentiallyExpirable<User>());
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<User>(10));
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<Struct>(5));
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<Uri2>(10));
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<decimal>(5));
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<nuint>(15));

        Console.ReadLine();
    }

    private static void SeedRandomlyExpirable<T>(int millions) where T : notnull, new()
    {
        const int secondsMin = 60;
        const int secondsMax = 1800;
        const uint count = 100_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, millions * 10, static num =>
        {
            var sw = Stopwatch.StartNew();
            for (uint i = 0; i < count; i++)
            {
                var rand = TimeSpan.FromSeconds(Random.Shared.Next(secondsMin, secondsMax));

                new T().Cache(i, num, rand);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count / 10;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }

    private static void SeedSequentiallyExpirable<T>() where T : notnull, new()
    {
        const int countPerStep = 250_000;

        const int steps = 20;
        const int secondsMin = 30;
        const int secondsMax = 300;

        const int stepIncrement = (secondsMax - secondsMin) / steps;

        var sw = Stopwatch.StartNew();

        for (int i = 0; i < steps; i++)
        {
            var exp = TimeSpan.FromSeconds(secondsMin + (stepIncrement * i));

            for (int j = 0; j < countPerStep; j++)
            {
                new T().Cache(i, j, exp);
            }
        }

        const int count = countPerStep * steps;
        var elapsed = sw.Elapsed;
        var ticksPerItem = elapsed.Ticks / (double)count / 10;
        Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
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
