using System.Buffers;
using FastCache.Services;
using FastCache.Utils;
using NonBlocking;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_store = new();
    internal static readonly EvictionJob<T> s_evictionJob = new();
    internal static readonly QuickEvictList<T> s_quickEvictList = new();

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);
}

internal sealed class QuickEvictList<T> where T : notnull
{
    private long _count;

    public (int, int)[] Entries { get; private set; }

    public int Count => (int)Interlocked.Read(ref _count);

    public QuickEvictList()
    {
        Entries = ArrayPool<(int, int)>.Shared.Rent(Constants.CacheBufferSize);
        _count = 0;
    }

    public void Add(int value, int expiresAtTicks)
    {
        if (Count < Entries.Length)
        {
            lock (Entries)
            {
                Entries[Count] = (value, expiresAtTicks);
                Interlocked.Increment(ref _count);
            }
        }
    }

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    public void Replace((int, int)[] entries, int count)
    {
        Entries = entries;
        _count = count;
    }
}

internal sealed class EvictionJob<T> where T : notnull
{
    public readonly Timer QuickListEvictionTimer;
    public readonly Timer FullEvictionTimer;
    public readonly SemaphoreSlim FullEvictionLock = new(1, 1);

    public int EvictionBackoffCount;
    public int EvictionGCNotificationsCount;

    public EvictionJob()
    {
        if (Constants.DisableEviction)
        {
            QuickListEvictionTimer = new Timer(_ => { });
            FullEvictionTimer = new Timer(_ => { });
            return;
        }

        QuickListEvictionTimer = new(
            static _ => CacheManager.EvictFromQuickList<T>(Environment.TickCount),
            null,
            Constants.QuickListEvictionInterval,
            Constants.QuickListEvictionInterval);

        // Full eviction interval is always computed with jitter. Store to local so that start and repeat intervals are equal.
        var fullEvictionInterval = Constants.FullEvictionInterval;
        FullEvictionTimer = new(
            static _ => CacheManager.QueueFullEviction<T>(triggeredByTimer: true),
            null,
            fullEvictionInterval,
            fullEvictionInterval);

        Gen2GcCallback.Register(static () => CacheManager.QueueFullEviction<T>(triggeredByTimer: false));
    }
}
