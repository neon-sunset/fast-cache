using FastCache.Collections;
using FastCache.Extensions;
using FastCache.Services;
using FastCache.Helpers;

namespace FastCache.CachedTests.Internals;

public sealed class CacheManagerTests
{
    private record ExpiredEntry(string Value);
    private record OptionallyExpiredEntry(string Value, bool IsExpired);
    private record RemovableEntry(string Value);

    private static readonly Random Random = new();
    private static readonly TimeSpan DelayTolerance = TimeSpan.FromMilliseconds(100);

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
        CacheManager.SuspendEviction<int, ExpiredEntry>();

        var quickList = CacheStaticHolder<int, ExpiredEntry>.QuickList;
        var store = CacheStaticHolder<int, ExpiredEntry>.Store;

        var length = Constants.QuickListMinLength * 2;
        var entries = Enumerable.Range(0, length).Select(i => (i, new ExpiredEntry(GetRandomString())));

        CachedRange<ExpiredEntry>.Save(entries, DelayTolerance.DivideBy(2));

        await Task.Delay(DelayTolerance);

        Assert.Equal((uint)Constants.QuickListMinLength, quickList.AtomicCount);
        Assert.Equal(length, store.Count);

        await Task.Delay(DelayTolerance);

        CacheManager.ResumeEviction<int, ExpiredEntry>();
        CacheManager.QueueFullEviction<int, ExpiredEntry>();

        await Task.Delay(DelayTolerance);

        Assert.Equal(0u, quickList.AtomicCount);
        Assert.Empty(store);
    }

    [Fact]
    public async Task QueueFullEviction_CorrectlyEvictsEntries_SomeExpired()
    {
        CacheManager.SuspendEviction<int, OptionallyExpiredEntry>();

        var quickList = CacheStaticHolder<int, OptionallyExpiredEntry>.QuickList;
        var store = CacheStaticHolder<int, OptionallyExpiredEntry>.Store;

        var length = Constants.QuickListMinLength * 2;

        for (var i = 0; i < length; i++)
        {
            _ = i % 2 is 0
                ? new OptionallyExpiredEntry(GetRandomString(), IsExpired: true).Cache(i, DelayTolerance.DivideBy(2))
                : new OptionallyExpiredEntry(GetRandomString(), IsExpired: false).Cache(i, TimeSpan.MaxValue);
        }

        Assert.Equal((uint)Constants.QuickListMinLength, quickList.AtomicCount);
        Assert.Equal(length, store.Count);

        await Task.Delay(DelayTolerance);

        CacheManager.ResumeEviction<int, OptionallyExpiredEntry>();
        CacheManager.QueueFullEviction<int, OptionallyExpiredEntry>();

        await Task.Delay(DelayTolerance);

        Assert.Equal((uint)length / 2, quickList.AtomicCount);
        Assert.Equal(length / 2, store.Count);

        foreach (var (_, inner) in store)
        {
            Assert.False(inner.Value.IsExpired);
        }
    }

    [Fact]
    public async Task QueueFullClear_Clears()
    {
        CacheManager.SuspendEviction<string, RemovableEntry>();

        var quickList = CacheStaticHolder<string, RemovableEntry>.QuickList;
        var store = CacheStaticHolder<string, RemovableEntry>.Store;

        var length = Constants.QuickListMinLength * 2;
        for (var i = 0; i < length; i++)
        {
            var expiration = TimeSpan.FromMilliseconds(Random.Next(1, int.MaxValue));

            new RemovableEntry(GetRandomString()).Cache(i.ToString(), expiration);
        }

        Assert.Equal((uint)Constants.QuickListMinLength, quickList.AtomicCount);
        Assert.Equal(length, store.Count);

        CacheManager.QueueFullClear<string, RemovableEntry>();

        await Task.Delay(DelayTolerance);

        Assert.Equal(0u, quickList.AtomicCount);
        Assert.Empty(store);
    }
}
