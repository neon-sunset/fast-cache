using System.Buffers;
using System.Collections.Concurrent;
using FastCache.Jobs;
using FastCache.Utils;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();

    internal static readonly JobHolder<T> s_quickListEvictionJob = new(
        new Timer(
            static _ => CacheEvictionJob.EvictFromQuickList<T>(DateTime.UtcNow.Ticks),
            null,
            Constants.QuickListEvictionInterval,
            Constants.QuickListEvictionInterval));

    internal static readonly CachedQuickEvictList<T> s_quickEvictList = new();

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);
}

internal sealed class CachedQuickEvictList<T> where T : notnull
{
    public (int, long)[] Entries { get; private set; }

    public int Count { get; private set; }

    public CachedQuickEvictList()
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
    public readonly Timer Timer;

    public readonly SemaphoreSlim InstanceLock = new(2, 2);

    public JobHolder(Timer timer)
    {
        Timer = timer;

        Gen2GcCallback.Register(static () => CacheEvictionJob.QueueFullEviction<T>());
    }
}
