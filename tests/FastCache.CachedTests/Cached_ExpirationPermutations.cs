using FastCache.Extensions;

namespace FastCache.CachedTests;

public sealed class CachedTests_ExpirationPermutations
{
    [Theory]
    [InlineData(10)]
    [InlineData(250)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(2500)]
    public async Task Cached_TryGet_ReturnsValueBeforeExpiration_DoesNotReturnAfter(int milliseconds)
    {
        const int delayTolerance = 100;

        var key = GetTestKey(milliseconds);
        var value = GetRandomString();
        value.Cache(key, TimeSpan.FromMilliseconds(milliseconds));

        await Task.Delay(Math.Max(0, milliseconds - delayTolerance));
        var foundBefore = Cached<string>.TryGet(key, out var cachedBefore);

        Assert.True(foundBefore);
        Assert.Equal(value, cachedBefore.Value);

        await Task.Delay(delayTolerance + 1);
        var foundAfter = Cached<string>.TryGet(key, out _);

        Assert.False(foundAfter);
    }
}
