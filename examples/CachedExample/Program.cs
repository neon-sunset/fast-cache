using CachedExample;
using FastCache.Services;

await GetOrCompute.Run();

// Initialize();

// CacheManager.QueueFullEviction<string>();

// Console.ReadLine();

// void Initialize()
// {
//     const int parallelism = 1;
//     const int limit = 32000 / parallelism;
//     Parallel.For(0, parallelism, static num => Seed(num, limit));

//     static void Seed(int num, int limit)
//     {
//         // var ticksMax = TimeSpan.FromSeconds(30).Ticks;

//         for (int i = 0; i < limit; i++)
//         {
//             // var expiration = TimeSpan.FromTicks(Random.Shared.NextInt64(0, ticksMax));
//             $"string value of {i} and {num}".Cache(i, TimeSpan.Zero);
//         }
//     }
// }
