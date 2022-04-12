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

        var now = DateTime.UtcNow.Ticks;

        if (RemoveOldestEntries<T>(now))
        {
            Cached<T>.s_removeExpiredJob.InstanceLock.Release();
            return;
        }

        var store = Cached<T>.s_cachedStore;
        var expiredEntries = ArrayPool<int>.Shared.Rent(Constants.CacheBufferSize * 2);
        var expiredEntriesLength = expiredEntries.Length;

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (now > cacheItem.ExpiresAtTicks)
                {
                    expiredEntries[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;

                    if (expiredEntriesCount >= expiredEntriesLength)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(expiredEntries[i], out _);
            }

            if (expiredEntriesCount < expiredEntriesLength)
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
        var entriesSurvivedIndexes = arrPool.Rent(Constants.CacheBufferSize);

        lock (oldestEntries)
        {
            var entriesRemoved = 0;
            var entriesSurvived = 0;

            for (var i = 0; i < oldestEntries.Count; i++)
            {
                var (identifier, expiresAtTicks) = oldestEntries[i];

                if (now > expiresAtTicks)
                {
                    store.Remove(identifier, out _);
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

                arrPool.Return(entriesSurvivedIndexes);
                return continueEviction;
            }

            if (entriesRemoved == 0)
            {
                arrPool.Return(entriesSurvivedIndexes);
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
            arrPool.Return(entriesSurvivedIndexes);
            return continueEviction;
        }
    }
}
