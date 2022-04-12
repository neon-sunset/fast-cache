using System.Buffers;

namespace FastCache.Jobs;

internal static class CacheItemsEvictionJob
{
    public static bool Run<T>() where T : notnull
    {
        return ThreadPool.QueueUserWorkItem(static _ => RunInternal<T>());
    }

    public static void RunInternal<T>() where T : notnull
    {
        if (!Cached<T>.s_removeExpiredJob.InstanceLock.Wait(0))
        {
            return;
        }

        var bufferSize = Constants.CacheBufferSize;
        var now = DateTime.UtcNow.Ticks;

        if (RemoveOldestEntries<T>(now))
        {
            Cached<T>.s_removeExpiredJob.InstanceLock.Release();
            return;
        }

        var store = Cached<T>.s_cachedStore;
        var expiredEntries = ArrayPool<int>.Shared.Rent(bufferSize);

        var totalExpired = 0;

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (now > cacheItem.ExpiresAtTicks)
                {
                    expiredEntries[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;
                    totalExpired++;

                    if (expiredEntriesCount >= bufferSize)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(expiredEntries[i], out _);
            }

            if (expiredEntriesCount < bufferSize)
            {
                break;
            }
        }

        ArrayPool<int>.Shared.Return(expiredEntries);
        Cached<T>.s_removeExpiredJob.InstanceLock.Release();
    }

    private static bool RemoveOldestEntries<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var oldestEntries = Cached<T>.s_oldestEntries.Entries;
        var continueEviction = store.Count < Constants.CacheBufferSize;

        var arrPool = ArrayPool<int>.Shared;
        var removedIndexesBuf = arrPool.Rent(Constants.CacheBufferSize);
        var survivedIndexesBuf = arrPool.Rent(Constants.CacheBufferSize);

        lock (oldestEntries)
        {
            var entriesRemoved = 0;
            var entriesRemovedIndexes = ((Span<int>)removedIndexesBuf)[..Constants.CacheBufferSize];

            var entriesSurvived = 0;
            var entriesSurvivedIndexes = ((Span<int>)survivedIndexesBuf)[..Constants.CacheBufferSize];

            for (var i = 0; i < oldestEntries.Count; i++)
            {
                var (identifier, expiresAtTicks) = oldestEntries[i];

                if (now > expiresAtTicks)
                {
                    store.Remove(identifier, out _);
                    entriesRemovedIndexes[entriesRemoved] = i;
                    entriesRemoved++;
                }
                else
                {
                    entriesSurvivedIndexes[entriesSurvived] = i;
                    entriesSurvived++;
                }
            }

            if (entriesSurvived == 0)
            {
                oldestEntries.Clear();

                arrPool.Return(removedIndexesBuf);
                arrPool.Return(survivedIndexesBuf);
                return continueEviction;
            }

            if (entriesRemoved == 0)
            {
                arrPool.Return(removedIndexesBuf);
                arrPool.Return(survivedIndexesBuf);
                return continueEviction;
            }

            var updatedOldestEntries = new List<(int, long)>(Constants.CacheBufferSize);
            for (var j = 0; j < entriesSurvived; j++)
            {
                var entryIndex = entriesSurvivedIndexes[j];
                var entry = oldestEntries[entryIndex];
                updatedOldestEntries.Add(entry);
            }

            Cached<T>.s_oldestEntries = new(updatedOldestEntries);
            arrPool.Return(removedIndexesBuf);
            arrPool.Return(survivedIndexesBuf);
            return continueEviction;
        }
    }
}
