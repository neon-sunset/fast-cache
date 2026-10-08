using FastCache.Services;

namespace FastCache.CachedTests.Internals;

public sealed class CacheManagerClearRegressionTests
{
    private sealed record ShrinkingEntry;
    private sealed record EmptyTrimEntry;

    private static readonly TimeSpan OperationTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ClearTimeout = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan EntryLifetime = TimeSpan.FromHours(1);

    [Fact]
    public async Task ExecuteFullClear_AfterQuickListShrinks_CompletesWithoutEvictionFailure()
    {
        CacheManager.SuspendEviction<int, ShrinkingEntry>();
        var store = CacheStaticHolder<int, ShrinkingEntry>.Store;
        var job = CacheStaticHolder<int, ShrinkingEntry>.EvictionJob;

        try
        {
            // With default settings: 65,536 entries grow the quick list to 4,096.
            // Halving Store then shrinks the backing array to 2,048. Both requested
            // lengths must exceed QuickListMinLength to take the resizing branch.
            Assert.True(Constants.QuickListAdjustableLengthPercentage > 0);
            var initialCount = checked((int)Math.Ceiling(
                Constants.QuickListMinLength * 320d / Constants.QuickListAdjustableLengthPercentage));
            var entry = new ShrinkingEntry();
            for (var key = 0; key < initialCount; key++)
            {
                Cached<ShrinkingEntry>.Save(key, entry, EntryLifetime);
            }

            await StartEviction<ShrinkingEntry>().WaitAsync(OperationTimeout);

            for (var key = initialCount / 2; key < initialCount; key++)
            {
                Assert.True(Cached<ShrinkingEntry>.TryGet(key, out var cached));
                cached.Remove();
            }

            // Removed keys remain in the quick list with unexpired timestamps.
            // The old implementation copies only what fits into the smaller array,
            // but retains the old count, making the next eviction index past its end.
            await StartEviction<ShrinkingEntry>().WaitAsync(OperationTimeout);
            await StartEviction<ShrinkingEntry>().WaitAsync(OperationTimeout);

            Assert.Equal(1, job.FullEvictionLock.CurrentCount);
            Assert.False(CacheStaticHolder<int, ShrinkingEntry>.QuickList.InProgress);
            await CacheManager.ExecuteFullClear<int, ShrinkingEntry>().WaitAsync(ClearTimeout);
            Assert.Empty(store);
            Assert.Equal(0u, CacheStaticHolder<int, ShrinkingEntry>.QuickList.AtomicCount);
        }
        finally
        {
            CacheManager.SuspendEviction<int, ShrinkingEntry>();
            // Do not call Clear/Reset in cleanup: on the buggy implementation both
            // semaphores are leaked. This private value type isolates that state.
            store.Clear();
        }
    }

    [Fact]
    public async Task ExecuteFullClear_AfterTrimmingEmptyCache_Completes()
    {
        CacheManager.SuspendEviction<int, EmptyTrimEntry>();
        var quickList = CacheStaticHolder<int, EmptyTrimEntry>.QuickList;
        var store = CacheStaticHolder<int, EmptyTrimEntry>.Store;

        try
        {
            Assert.Empty(store);
            Assert.Equal(0u, quickList.AtomicCount);

            CacheManager.Trim<int, EmptyTrimEntry>(10);

            // Check the leak before launching Clear: Reset uses a synchronous Wait,
            // so timing out Clear alone would leave a ThreadPool worker blocked forever.
            Assert.False(quickList.InProgress, "Trimming an empty quick list must release its semaphore.");
            await CacheManager.ExecuteFullClear<int, EmptyTrimEntry>().WaitAsync(ClearTimeout);
            Assert.Empty(store);
        }
        finally
        {
            CacheManager.SuspendEviction<int, EmptyTrimEntry>();
            store.Clear();
        }
    }

    private static Task StartEviction<T>()
    {
        Assert.False(Constants.DisableEvictionJob, "These tests require automatic eviction to be enabled in configuration.");
        CacheManager.ResumeEviction<int, T>();
        try
        {
            // Execute the actual library path; suspend subsequent timer/GC triggers
            // immediately after dispatch. Suspension does not cancel in-flight work.
            return CacheManager.ExecuteFullEviction<int, T>();
        }
        finally
        {
            CacheManager.SuspendEviction<int, T>();
        }
    }
}
