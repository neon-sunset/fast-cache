using System.Collections.Concurrent;

namespace FastCache;

public partial struct Cached<T>
{
    internal static readonly ConcurrentDictionary<int, CachedInner<T>> s_cachedStore = new();

    internal static CachedOldestEntries<T> s_oldestEntries = new(new(Constants.OldestEntriesLimit));

    internal static (bool IsStored, CachedInner<T> Inner) s_default = (false, default);

    // internal static readonly (Timer, T) s_removeExpiredJob = (
    //     new Timer(
    //         static _ => RemoveExpiredEntriesJob.Run<T>(),
    //         null,
    //         TimeSpan.FromSeconds(10),
    //         TimeSpan.FromSeconds(10)),
    //     default!);
}

internal readonly struct CachedOldestEntries<T> where T : notnull
{
    public readonly List<(int, long)> Entries;

    public CachedOldestEntries(List<(int, long)> entries)
    {
        Entries = entries;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int value, long expiresAtTicks)
    {
        lock (Entries)
        {
            if (Entries.Count < Constants.OldestEntriesLimit)
            {
                Entries.Add((value, expiresAtTicks));
            }
        }
    }
}
