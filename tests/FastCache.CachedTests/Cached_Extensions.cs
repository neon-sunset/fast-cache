using FastCache.Extensions;

namespace FastCache.CachedTests;

public sealed class CachedTests_Extensions
{
    [Fact]
    public void CacheK1_Caches()
    {
        var key = GetTestKey();
        var value = GetRandomString();

        value.Cache(key, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(key, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK2_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK3_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, k3, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, k3, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK4_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, k3, k4, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, k3, k4, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK5_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, k3, k4, k5, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK6_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, k3, k4, k5, k6, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }

    [Fact]
    public void CacheK7_Caches()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var value = GetRandomString();

        value.Cache(k1, k2, k3, k4, k5, k6, k7, TimeSpan.MaxValue);

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var cached);

        Assert.True(found);
        Assert.Equal(value, cached.Value);
    }
}
