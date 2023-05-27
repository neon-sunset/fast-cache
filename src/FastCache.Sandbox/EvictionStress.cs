using System.Diagnostics;
using System.Security.Cryptography;
using FastCache.Collections;
using FastCache.Extensions;

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
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<Uri2>(10));
        // Thread.Sleep(250);
        // CacheManager.QueueFullClear<(uint, string, int, string, char, bool, float), Uri2>();
        ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<Struct>(5));
        ThreadPool.QueueUserWorkItem(_ => SeedSequentiallyExpirable<long>());
        // ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<User>(1));
        ThreadPool.QueueUserWorkItem(_ => SeedRandomlyExpirable<bool>(25));
        // ThreadPool.QueueUserWorkItem(_ => SeedSequentiallyExpirable<Uri2>());
        // ThreadPool.QueueUserWorkItem(_ => SeedIndefinite<Uri2>(10));

        Console.ReadLine();
    }

    private static void SeedRandomlyExpirable<T>(int millions) where T : new()
    {
        const int secondsMin = 1;
        const int secondsMax = 900;
        const int count = 100_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, millions * 10, static num =>
        {
            var typeName = typeof(T).Name;
            var sw = Stopwatch.StartNew();

            foreach (var i in 0..count)
            {
                var rand = TimeSpan.FromSeconds(Random.Shared.Next(secondsMin, secondsMax));

                _ = new T().Cache(i, num, rand);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count / 10;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }

    private static void SeedSequentiallyExpirable<T>() where T : new()
    {
        const int countPerStep = 1_000_000;

        const int steps = 20;
        const int secondsMin = 1;
        const int secondsMax = 600;

        const int stepIncrement = (secondsMax - secondsMin) / steps;

        var sw = Stopwatch.StartNew();

        foreach (var step in 0..steps)
        {
            var expiration = TimeSpan.FromSeconds(secondsMin + (stepIncrement * step));
            var values = (0..countPerStep).Select(i => ((step, i), new T()));

            CachedRange<T>.Save(values, expiration);
        }

        const int count = countPerStep * steps;
        var elapsed = sw.Elapsed;
        var ticksPerItem = elapsed.Ticks / (double)count / 10;
        Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
    }

    private static void SeedIndefinite<T>(int millions) where T : new()
    {
        const uint count = 1_000_000;

        // CacheManager.SuspendEviction<T>();

        Parallel.For(0, millions, static num =>
        {
            var expiration = TimeSpan.FromHours(24);
            var sw = Stopwatch.StartNew();
            for (uint i = 0; i < count; i++)
            {
                new T().Cache(i, "val2", num, "val4", '!', true, 0.1337f, expiration);
            }

            var elapsed = sw.Elapsed;
            var ticksPerItem = elapsed.Ticks / (double)count / 10;
            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache. Took {elapsed}, {ticksPerItem} us per item");
        });

        // Thread.Sleep(10000);

        // CacheManager.QueueFullEviction<T>();
    }
}
