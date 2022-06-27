using FastCache.Extensions;

namespace FastCache.Cached.Tests;

public sealed class Cached_ExpirationPermutations
{
    [Theory]
    [InlineData(5)]
    [InlineData(250)]
    [InlineData(1000)]
    [InlineData(5000)]
    public async Task Cached_TryGet_ReturnsValueAccordingToItemExpiration(int milliseconds)
    {
        var key = RandomString();
        var value = RandomString();
        value.Cache(key, TimeSpan.FromMilliseconds(milliseconds));

        var foundBefore = Cached<string>.TryGet(key, out var cachedBefore);
        await Task.Delay(milliseconds + 1);
        var foundAfter = Cached<string>.TryGet(key, out _);

        Assert.True(foundBefore);
        Assert.Equal(value, cachedBefore.Value);

        Assert.False(foundAfter);
    }
}
