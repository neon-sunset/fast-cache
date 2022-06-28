namespace FastCache.CachedTests;

public sealed class CachedTests_ParamsPermutations
{
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
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
        var foundValue = afterSave.Value;

        Assert.True(found);
        Assert.Equal(value, foundValue);
    }
}
