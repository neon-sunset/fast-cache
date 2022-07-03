using FastCache.Collections;
using FastCache.Extensions;
using FastCache.Services;

namespace FastCache.CachedTests.Internals;

public sealed class CacheManagerTests
{
    private record ExpiredEntry(string Value);
    private record OptionallyExpiredEntry(string Value, bool IsExpired);

    private static readonly TimeSpan EvictionDelayTolerance = TimeSpan.FromMilliseconds(500);

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.MinValue)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(-double.Epsilon)]
    [InlineData(100.00001)]
    [InlineData(100.1D)]
    [InlineData(double.MaxValue)]
    [InlineData(double.PositiveInfinity)]
    public void Trim_Throws_OnInvalidPercentage(double percentage)
    {
        void Trim() => CacheManager.Trim<int, object>(percentage);

        Assert.Throws<ArgumentOutOfRangeException>(Trim);
    }

    [Fact]
    public async Task QueueFullEviction_CorrectlyEvictsEntries_AllExpired()
    {
        var quickList = CacheStaticHolder<int, ExpiredEntry>.QuickList;
        var store = CacheStaticHolder<int, ExpiredEntry>.Store;

        var length = Constants.QuickListMinLength * 2;
        var entries = Enumerable.Range(0, length).Select(i => (i, new ExpiredEntry(GetRandomString())));

        CachedRange<ExpiredEntry>.Save(entries, TimeSpan.FromMilliseconds(250));

        Assert.Equal((uint)Constants.QuickListMinLength, quickList.AtomicCount);
        Assert.Equal(length, store.Count);

        await Task.Delay(250);

        CacheManager.QueueFullEviction<int, ExpiredEntry>();

        await Task.Delay(EvictionDelayTolerance);

        Assert.Equal(0u, quickList.AtomicCount);
        Assert.Empty(store);
    }

    [Fact]
    public async Task QueueFullEviction_CorrectlyEvictsEntries_SomeExpired()
    {
        var quickList = CacheStaticHolder<int, OptionallyExpiredEntry>.QuickList;
        var store = CacheStaticHolder<int, OptionallyExpiredEntry>.Store;

        var length = Constants.QuickListMinLength * 2;

        for (var i = 0; i < length; i++)
        {
            _ = i % 2 is 0
                ? new OptionallyExpiredEntry(GetRandomString(), IsExpired: true).Cache(i, TimeSpan.FromMilliseconds(250))
                : new OptionallyExpiredEntry(GetRandomString(), IsExpired: false).Cache(i, TimeSpan.MaxValue);
        }

        Assert.Equal((uint)Constants.QuickListMinLength, quickList.AtomicCount);
        Assert.Equal(length, store.Count);

        await Task.Delay(250);

        CacheManager.QueueFullEviction<int, OptionallyExpiredEntry>();

        await Task.Delay(EvictionDelayTolerance);

        Assert.Equal((uint)length / 2, quickList.AtomicCount);
        Assert.Equal(length / 2, store.Count);

        foreach (var (_, inner) in store)
        {
            Assert.False(inner.Value.IsExpired);
        }
    }
}
