using FastCache.Extensions;

namespace FastCache.CachedTests;

public sealed class CachedTests_TryGetAndSave
{
    private record InlineTrimItem(string Value);
    private record NonInlineTrimItem(string Value);

    private static readonly TimeSpan Expiration = TimeSpan.MaxValue;

    [Fact]
    public void Cached_NotSaved_ReturnsFalse()
    {
        var key = GetTestKey();

        var found = Cached<string>.TryGet(key, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue()
    {
        var key = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(key, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(key, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_SaveWithLimit_TrimsOnMaxCapacity_And_ReplacesValue()
    {
        var limit = (int)(Constants.InlineTrimCountThreshold * (100.0 / Constants.FullCapacityTrimPercentage));

        for (uint i = 0; i < limit; i++)
        {
            var item = new InlineTrimItem(GetRandomString());

            item.Cache(GetTestKey(i), TimeSpan.MaxValue);
        }

        Assert.Equal(limit, CacheStaticHolder<string, InlineTrimItem>.Store.Count);

        var key = GetTestKey(-1);
        var foundBefore = Cached<InlineTrimItem>.TryGet(key, out var cached);
        var value = cached.Save(new InlineTrimItem(GetRandomString()), TimeSpan.MaxValue, (uint)limit);

        Assert.False(foundBefore);

        var foundAfter = Cached<InlineTrimItem>.TryGet(key, out var cachedAfter);

        Assert.True(foundAfter);
        Assert.Equal(value, cachedAfter);
        Assert.True(CacheStaticHolder<string, InlineTrimItem>.Store.Count < limit);
    }

    [Fact]
    public async Task Cached_SaveWithLimit_DoesNotSaveOnOverCapacity_And_OverInlineTrimThreshold()
    {
        var limit = (int)(Constants.QuickListMinLength * (100.0 / Constants.FullCapacityTrimPercentage)) + Constants.InlineTrimCountThreshold;

        for (uint i = 0; i < limit; i++)
        {
            var item = new NonInlineTrimItem(GetRandomString());

            item.Cache(GetTestKey(i), TimeSpan.MaxValue);
        }

        Assert.Equal(limit, CacheStaticHolder<string, NonInlineTrimItem>.Store.Count);

        var key = GetTestKey(-1);
        var foundBefore = Cached<NonInlineTrimItem>.TryGet(key, out var cached);
        cached.Save(new NonInlineTrimItem(GetRandomString()), TimeSpan.MaxValue, (uint)limit);

        Assert.False(foundBefore);

        var foundAfter = Cached<NonInlineTrimItem>.TryGet(key, out _);

        Assert.False(foundAfter);

        await Task.Delay(500);
        Assert.True(CacheStaticHolder<string, NonInlineTrimItem>.Store.Count < limit);
    }

    [Fact]
    public void Cached_Remove_Removes()
    {
        var key = GetTestKey();
        var value = GetRandomString();

        value.Cache(key, TimeSpan.MaxValue);

        var foundBefore = Cached<string>.TryGet(key, out var cached);

        Assert.True(foundBefore);
        Assert.Equal(value, cached);

        cached.Remove();

        var foundAfter = Cached<string>.TryGet(key, out _);

        Assert.False(foundAfter);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K2()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K2()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K3()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, k3, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K3()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, k3, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, k3, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K4()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K4()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, k3, k4, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, k3, k4, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K5()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K5()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, k3, k4, k5, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K6()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K6()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }

    [Fact]
    public void Cached_NotSaved_ReturnsFalse_K7()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K7()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var value = GetRandomString();

        if (!Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var beforeSave))
        {
            beforeSave.Save(value, Expiration);
        }

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var afterSave);

        Assert.True(found);
        Assert.Equal(value, afterSave);
    }
}
