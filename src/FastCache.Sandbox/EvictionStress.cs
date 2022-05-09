using System.Security.Cryptography;

namespace FastCache.Sandbox;

public static class EvictionStress
{
    private record User(string Login, string Password, string Name, string Email, DateTime DateOfBirth)
    {
        public User() : this(
            $"Login {RandomNumberGenerator.GetInt32(0, int.MaxValue)}",
            $"Password {RandomNumberGenerator.GetInt32(0, int.MaxValue)}",
            $"Name {RandomNumberGenerator.GetInt32(0, int.MaxValue)}",
            $"Email {RandomNumberGenerator.GetInt32(0, int.MaxValue)}",
            DateTime.Now) { }
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
        // ThreadPool.QueueUserWorkItem(_ => Seed<Struct>());
        // ThreadPool.QueueUserWorkItem(_ => Seed<Uri2>());
        // ThreadPool.QueueUserWorkItem(_ => Seed<decimal>());
        // ThreadPool.QueueUserWorkItem(_ => Seed<nuint>());
        Console.ReadLine();
    }

    private static void Seed<T>() where T : notnull, new()
    {
        const int count = 2_000_000;

        Parallel.For(0, 25, static num =>
        {
            var offset = num * count;
            for (int i = offset; i < count + offset; i++)
            {
                var rand = TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(1, 900));

                new T().Cache(i, rand);
            }

            Console.WriteLine($"Added {count} of {typeof(T).Name} to cache.");
        });
    }
}
