using FastCache.Services;
using FastCache.Utils;

namespace FastCache;

internal sealed class EvictionJob<T> where T : notnull
{
    private readonly Timer _quickListEvictionTimer;
    private readonly Timer _fullEvictionTimer;

    private ulong _averageExpirationMilliseconds;
    private bool _active = true;

    public readonly SemaphoreSlim FullEvictionLock = new(1, 1);

    public int EvictionGCNotificationsCount;

    public EvictionJob()
    {
        if (Constants.DisableEvictionJob)
        {
            _quickListEvictionTimer = new Timer(_ => { });
            _fullEvictionTimer = new Timer(_ => { });
            return;
        }

        _quickListEvictionTimer = new(
            static _ => Cached<T>.s_quickList.Evict(Environment.TickCount64),
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

    public void ReportExpiration(ulong milliseconds)
    {
        var previous = _averageExpirationMilliseconds;

        _averageExpirationMilliseconds = (milliseconds + previous) / 2;
    }

    public void RescheduleConsideringExpiration()
    {
        if (!_active)
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
        }

        var newFullEvictionInterval = Constants.FullEvictionInterval;
        var adjustedFullEvictionInterval = (averageExpiration + newFullEvictionInterval) / 2;
        if (adjustedFullEvictionInterval > newFullEvictionInterval)
        {
            _fullEvictionTimer.Change(adjustedFullEvictionInterval, adjustedFullEvictionInterval);
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
