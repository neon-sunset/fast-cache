namespace FastCache.Jobs;

[SkipLocalsInit]
public static class RemoveExpiredEntriesJob
{
    public static void Run<T>() where T : notnull
    {
        const int stackLimit = Constants.ExpiredEntriesJobStackLimit;

        var now = DateTime.UtcNow.Ticks;

        if (RemoveOldestEntries<T>(now))
        {
            return;
        }

        var store = Cached<T>.s_cachedStore;
        var expiredEntries = (stackalloc int[stackLimit]);

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (now > cacheItem.ExpiresAtTicks)
                {
                    expiredEntries[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;

                    if (expiredEntriesCount >= stackLimit)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(expiredEntries[i], out _);
            }

            if (expiredEntriesCount < stackLimit)
            {
                break;
            }
        }
    }

    private static bool RemoveOldestEntries<T>(long now) where T : notnull
    {
        var store = Cached<T>.s_cachedStore;
        var oldestEntries = Cached<T>.s_oldestEntries.Entries;

        lock (oldestEntries)
        {
            var entriesRemoved = 0;
            var entriesRemovedIndexes = (stackalloc int[Constants.OldestEntriesLimit]);

            var entriesTotal = oldestEntries.Count;

            for (var i = 0; i < entriesTotal; i++)
            {
                var (identifier, expiresAtTicks) = oldestEntries[i];

                if (now > expiresAtTicks)
                {
                    store.Remove(identifier, out _);
                    entriesRemovedIndexes[entriesRemoved] = i;
                    entriesRemoved++;
                }
            }

            if (entriesRemoved == entriesTotal)
            {
                oldestEntries.Clear();

                return entriesRemoved < Constants.OldestEntriesLimit;
            }

            var updatedOldestEntries = new List<(int, long)>(Constants.OldestEntriesLimit);
            for (var j = 0; j < entriesRemoved; j++)
            {
                var entryIndex = entriesRemovedIndexes[j];
                var entry = oldestEntries[entryIndex];
                updatedOldestEntries.Add(entry);
            }

            Cached<T>.s_oldestEntries = new(updatedOldestEntries);
            return false;
        }
    }
}
