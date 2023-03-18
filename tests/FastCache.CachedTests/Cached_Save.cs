namespace FastCache.CachedTests;

public sealed class CachedTests_Save
{
    [Fact]
    public void SaveK1_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, out _);
        var returnValue = Cached<string>.Save(k1, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK1Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, out _);
        var returnValue = Cached<string>.Save(k1, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK2_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, out _);
        var returnValue = Cached<string>.Save(k1, k2, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK2Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, out _);
        var returnValue = Cached<string>.Save(k1, k2, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK3_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK3Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK4_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK4Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK5_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK5Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK6_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, k6, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK6Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, k6, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK7_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, k6, k7, value, TimeSpan.FromSeconds(30));
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }

    [Fact]
    public void SaveK7Limit_SavesCorrectly()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var value = GetRandomString();

        var foundBefore = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out _);
        var returnValue = Cached<string>.Save(k1, k2, k3, k4, k5, k6, k7, value, TimeSpan.FromSeconds(30), limit: int.MaxValue);
        var foundAfter = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.False(foundBefore);
        Assert.Equal(value, returnValue);
        Assert.Equal(value, cachedAfter);
    }
}
