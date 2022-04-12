using System.Collections.Concurrent;
using FastCache.Jobs;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();

    internal static readonly JobHolder<T> s_removeExpiredJob = new(
        new Timer(
            static _ => CacheItemsEvictionJob.Run<T>(),
            null,
            Constants.CacheItemsEvictionInterval,
            Constants.CacheItemsEvictionInterval));

    internal static CachedOldestEntries<T> s_oldestEntries = new(new(Constants.CacheBufferSize));

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);
}

internal readonly struct CachedOldestEntries<T> where T : notnull
{
    public readonly List<(int, long)> Entries;

    public CachedOldestEntries(List<(int, long)> entries)
    {
        Entries = entries;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int value, long expiresAtTicks)
    {
        lock (Entries)
        {
            if (Entries.Count < Constants.CacheBufferSize)
            {
                Entries.Add((value, expiresAtTicks));
            }
        }
    }
}

internal sealed class JobHolder<T>
{
    public readonly Timer Timer;

    public readonly SemaphoreSlim InstanceLock = new(2, 2);

    public JobHolder(Timer timer)
    {
        Timer = timer;
    }
}
