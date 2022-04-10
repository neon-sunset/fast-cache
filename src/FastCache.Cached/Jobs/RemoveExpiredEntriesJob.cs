namespace FastCache.Jobs;

[SkipLocalsInit]
public static class RemoveExpiredEntriesJob
{
    public static void Run<T>() where T : notnull
    {
        var now = DateTime.UtcNow.Ticks;
        var store = Cached<T>.s_cachedStore;

        var expiredEntries = (stackalloc int[4096]);

        while (true)
        {
            var expiredEntriesCount = 0;

            foreach (var (identifier, cacheItem) in store)
            {
                if (cacheItem.ExpiresAtTicks < now)
                {
                    expiredEntries[expiredEntriesCount] = identifier;
                    expiredEntriesCount++;

                    if (expiredEntriesCount >= 4096)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < expiredEntriesCount; i++)
            {
                store.Remove(expiredEntries[i], out _);
            }

            if (expiredEntriesCount < 4096)
            {
                break;
            }
        }
    }
}
