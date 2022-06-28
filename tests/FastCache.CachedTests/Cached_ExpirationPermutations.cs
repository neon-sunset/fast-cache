using FastCache.Extensions;

namespace FastCache.CachedTests;

public sealed class CachedTests_ExpirationPermutations
{
    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(2500)]
    public async Task Cached_TryGet_ReturnsValueAccordingToItemExpiration(int milliseconds)
    {
        const int delayTolerance = 50;

        var key = RandomString();
        var value = RandomString();
        value.Cache(key, TimeSpan.FromMilliseconds(milliseconds));

        await Task.Delay(milliseconds - delayTolerance);
        var foundBefore = Cached<string>.TryGet(key, out var cachedBefore);

        Assert.True(foundBefore);
        Assert.Equal(value, cachedBefore.Value);

        await Task.Delay(milliseconds);
        var foundAfter = Cached<string>.TryGet(key, out _);

        Assert.False(foundAfter);
    }
}
