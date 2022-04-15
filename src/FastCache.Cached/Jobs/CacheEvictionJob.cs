using System.Buffers;

namespace FastCache.Jobs;

public static class CacheEvictionJob
{
    public static bool QueueFullEviction<T>() where T : notnull
    {
        return ThreadPool.QueueUserWorkItem(static _ => EvictExpired<T>());
    }

    public static void EvictExpired<T>() where T : notnull
    {
        if (!Cached<T>.s_quickListEvictionJob.InstanceLock.Wait(0))
        {
            return;
        }

        var now = DateTime.UtcNow.Ticks;

        if (EvictFromQuickList<T>(now))
        {
            Cached<T>.s_quickListEvictionJob.InstanceLock.Release();
            return;
        }

        var store = Cached<T>.s_cachedStore;
        var buffer = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize * 2);
        var bufferLength = buffer.Length;

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (now > cacheItem.ExpiresAtTicks)
                {
                    buffer[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;

                    if (expiredEntriesCount >= bufferLength)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(buffer[i], out _);
            }

            if (expiredEntriesCount < bufferLength)
            {
                break;
            }
        }

        ArrayPool<int>.Shared.Return(buffer);
        Cached<T>.s_quickListEvictionJob.InstanceLock.Release();
    }

    /// <summary>
    /// Returns true if resident cache size is contained within quicklist and full eviction is not required.
    /// </summary>
    public static bool EvictFromQuickList<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var continueEviction = store.Count < Constants.CacheBufferSize;

        var entriesSurvivedIndexes = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize);

        lock (Cached<T>.s_quickEvictList.Entries)
        {
            var quickListEntries = Cached<T>.s_quickEvictList.Entries;
            var entriesCount = Cached<T>.s_quickEvictList.Count;

            var entriesRemovedCount = 0;
            var entriesSurvivedCount = 0;

            for (var i = 0; i < entriesCount; i++)
            {
                var (identifier, expiresAtTicks) = quickListEntries[i];

                if (now > expiresAtTicks)
                {
                    store.Remove(identifier, out _);
                    entriesRemovedCount++;
                }
                else
                {
                    entriesSurvivedIndexes[entriesSurvivedCount] = i;
                    entriesSurvivedCount++;
                }
            }

            if (entriesSurvivedCount == 0)
            {
                Cached<T>.s_quickEvictList.Reset();

                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
                return continueEviction;
            }

            if (entriesRemovedCount == 0)
            {
                ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
                return continueEviction;
            }

            var entriesSurvived = ArrayPool<(int, long)>.Shared.Rent(Constants.CacheBufferSize);
            for (var j = 0; j < entriesSurvivedCount; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                entriesSurvived[j] = quickListEntries[entryIndex];
            }

            Cached<T>.s_quickEvictList.Replace(entriesSurvived, entriesSurvivedCount);
            ArrayPool<(int, long)>.Shared.Return(quickListEntries);
            ArrayPool<int>.Shared.Return(entriesSurvivedIndexes);
            return continueEviction;
        }
    }
}
