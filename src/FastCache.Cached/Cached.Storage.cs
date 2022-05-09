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
    private (int, long)[] _active;
    private (int, long)[] _inactive;
    private long _count;

    public (int, long)[] Entries => _active;
    public (int, long)[] Inactive => _inactive;
    public int Count => (int)Interlocked.Read(ref _count);

    public QuickEvictList()
    {
        _active = new (int, long)[Constants.CacheBufferSize];
        _inactive = new (int, long)[Constants.CacheBufferSize];
        _count = 0;
    }

    public void Add(int value, long expiresAt)
    {
        var entries = Entries;
        var count = Count;
        if (count < entries.Length)
        {
            entries[count] = (value, expiresAt);

            Interlocked.CompareExchange(ref _count, count + 1, count);
        }
    }

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    public void SwapOnEviction(int survivedCount)
    {
        Interlocked.Exchange(ref _count, survivedCount);

        _inactive = Interlocked.Exchange(ref _active, _inactive);
    }
}

internal sealed class EvictionJob<T> where T : notnull
{
    private long _averageExpirationMilliseconds;

    public readonly Timer QuickListEvictionTimer;
    public readonly Timer FullEvictionTimer;
    public readonly SemaphoreSlim FullEvictionLock = new(1, 1);

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
            static _ => CacheManager.EvictFromQuickList<T>(Environment.TickCount64),
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

    public void ReportExpiration(long milliseconds)
    {
        var previous = _averageExpirationMilliseconds;

        _averageExpirationMilliseconds = (milliseconds + previous) / 2;
    }

    public void RescheduleTimers()
    {
        var milliseconds = Interlocked.Read(ref _averageExpirationMilliseconds);
        var averageExpiration = TimeSpan.FromMilliseconds(milliseconds);

        var adjustedQuicklistInterval =
            ((averageExpiration / Constants.EvictionIntervalMultiplyFactor) + Constants.QuickListEvictionInterval) / 2;

        if (adjustedQuicklistInterval > Constants.QuickListEvictionInterval)
        {
            QuickListEvictionTimer.Change(adjustedQuicklistInterval, adjustedQuicklistInterval);
        }

        var newFullEvictionInterval = Constants.FullEvictionInterval;
        var adjustedFullEvictionInterval = (averageExpiration + newFullEvictionInterval) / 2;
        if (adjustedFullEvictionInterval > newFullEvictionInterval)
        {
            FullEvictionTimer.Change(adjustedFullEvictionInterval, adjustedFullEvictionInterval);
        }
        else
        {
            FullEvictionTimer.Change(newFullEvictionInterval, newFullEvictionInterval);
        }
    }
}
