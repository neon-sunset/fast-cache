using System.Buffers;
using System.Collections.Concurrent;
using FastCache.Services;
using FastCache.Utils;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();
    internal static readonly JobHolder<T> s_evictionJob = new();
    internal static readonly QuickEvictList<T> s_quickEvictList = new();

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);
}

internal sealed class QuickEvictList<T> where T : notnull
{
    public (int, long)[] Entries { get; private set; }

    public int Count { get; private set; }

    public QuickEvictList()
    {
        Entries = ArrayPool<(int, long)>.Shared.Rent(Constants.CacheBufferSize);
        Count = 0;
    }

    public void Add(int value, long expiresAtTicks)
    {
        lock (Entries)
        {
            if (Count < Constants.CacheBufferSize)
            {
                Entries[Count] = (value, expiresAtTicks);
                Count++;
            }
        }
    }

    public void Reset() => Count = 0;

    public void Replace((int, long)[] entries, int count)
    {
        Entries = entries;
        Count = count;
    }
}

internal sealed class JobHolder<T> where T : notnull
{
    public readonly Timer QuickListEvictionTimer;
    public readonly Timer FullEvictionTimer;
    public readonly SemaphoreSlim FullEvictionLock = new(1, 1);

    public int ReportedEvictionsCount;

    public JobHolder()
    {
        QuickListEvictionTimer = new(
            static _ => CacheManager.EvictFromQuickList<T>(DateTime.UtcNow.Ticks),
            null,
            Constants.QuickListEvictionInterval,
            Constants.QuickListEvictionInterval);

        // Full eviction interval is always computed with jitter. Store to local so that start and repeat intervals are equal.
        var fullEvictionInterval = Constants.FullEvictionInterval;
        FullEvictionTimer = new(
            static _ => CacheManager.QueueFullEviction<T>(),
            null,
            fullEvictionInterval,
            fullEvictionInterval);

        Gen2GcCallback.Register(static () => CacheManager.QueueFullEviction<T>());
    }
}
