using FastCache.Extensions;

namespace FastCache.CachedTests;

public sealed class CachedTests_Update
{
    [Fact]
    public void Cached_Update_UpdatesValue()
    {
        var key = GetTestKey();
        var valueBefore = GetRandomString();
        valueBefore.Cache(key, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(key, out var cachedBefore);

        Assert.True(found);
        Assert.Equal(valueBefore, cachedBefore.Value);

        var valueAfter = GetRandomString();
        var updated = cachedBefore.Update(valueAfter);

        var foundAfter = Cached<string>.TryGet(key, out var cachedAfter);

        Assert.True(updated);
        Assert.True(foundAfter);
        Assert.Equal(valueAfter, cachedAfter.Value);
    }

    [Fact]
    public void Cached_Update_DoesNotUpdateMissingValue()
    {
        var key = GetTestKey();
        var value = GetRandomString();

        var found = Cached<string>.TryGet(key, out var cached);
        var updated = cached.Update(value);

        Assert.False(found);
        Assert.False(updated);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(2500)]
    public async Task Cached_Update_PreservesExpiration(int milliseconds)
    {
        const int delayTolerance = 100;

        var key = GetTestKey(milliseconds);
        var valueBefore = GetRandomString();
        var valueAfter = GetRandomString();

        valueBefore.Cache(key, TimeSpan.FromMilliseconds(milliseconds));

        await Task.Delay(milliseconds - delayTolerance);
        var updated = Cached<string>.TryGet(key, out var cached) && cached.Update(valueAfter);

        await Task.Delay(delayTolerance);
        var foundAfter = Cached<string>.TryGet(key, out _);

        Assert.True(updated);
        Assert.False(foundAfter);
    }
}
