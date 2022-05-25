using FastCache.Services;
using FastCache.Helpers;

namespace FastCache;

internal sealed class EvictionJob<T>
{
    private readonly Timer _quickListEvictionTimer;
    private readonly Timer _fullEvictionTimer;

    private long _averageExpirationMilliseconds;
    private bool _active = true;

    public bool IsActive => Volatile.Read(ref _active);

    public readonly SemaphoreSlim FullEvictionLock = new(1, 1);

    public int EvictionGCNotificationsCount;

    public EvictionJob()
    {
        if (Constants.DisableEvictionJob)
        {
            _quickListEvictionTimer = new Timer(_ => { });
            _fullEvictionTimer = new Timer(_ => { });
            _active = false;
            return;
        }

        _quickListEvictionTimer = new(
            static _ => Cached<T>.s_quickList.Evict(),
            null,
            Constants.QuickListEvictionInterval,
            Constants.QuickListEvictionInterval);

        // Full eviction interval is always computed with jitter. Store to local so that start and repeat intervals are equal.
        var fullEvictionInterval = Constants.FullEvictionInterval;
        _fullEvictionTimer = new(
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

    public void RescheduleConsideringExpiration()
    {
        if (!IsActive)
        {
            return;
        }

        var milliseconds = Interlocked.Read(ref _averageExpirationMilliseconds);
        var averageExpiration = TimeSpan.FromMilliseconds(milliseconds);

        var adjustedQuicklistInterval =
            ((averageExpiration / Constants.EvictionIntervalMultiplyFactor) + Constants.QuickListEvictionInterval) / 2;

        if (adjustedQuicklistInterval > Constants.QuickListEvictionInterval)
        {
            _quickListEvictionTimer.Change(adjustedQuicklistInterval, adjustedQuicklistInterval);
#if FASTCACHE_DEBUG
            Console.WriteLine($"FastCache: {typeof(T).Name} eviction interval from quick list has been rescheduled to {adjustedQuicklistInterval}.");
#endif
        }

        var newFullEvictionInterval = Constants.FullEvictionInterval;
        var adjustedFullEvictionInterval = (averageExpiration + newFullEvictionInterval) / 2;
        if (adjustedFullEvictionInterval > newFullEvictionInterval)
        {
            _fullEvictionTimer.Change(adjustedFullEvictionInterval, adjustedFullEvictionInterval);
#if FASTCACHE_DEBUG
            Console.WriteLine($"FastCache: {typeof(T).Name} eviction interval from cache store has been rescheduled to {adjustedFullEvictionInterval}.");
#endif
        }
        else
        {
            _fullEvictionTimer.Change(newFullEvictionInterval, newFullEvictionInterval);
        }
    }

    public void Resume()
    {
        if (Constants.DisableEvictionJob)
        {
            return;
        }

        Volatile.Write(ref _active, true);

        RescheduleConsideringExpiration();
    }

    public void Stop()
    {
        Volatile.Write(ref _active, false);

        _quickListEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _fullEvictionTimer.Change(Timeout.Infinite, Timeout.Infinite);
    }
}
