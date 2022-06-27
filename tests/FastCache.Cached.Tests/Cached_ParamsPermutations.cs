namespace FastCache.Cached.Tests;

public sealed class CachedTests_ParamsPermutations
{
    private static readonly TimeSpan Expiration = TimeSpan.MaxValue;

    [Fact]
    public void Cached_NotSaved_ReturnsFalse()
    {
        var key = RandomString();

        var found = Cached<string>.TryGet(key, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue()
    {
        var key = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K2()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, k3, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K3()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K4()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K5()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K6()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var value = RandomString();

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
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var k7 = RandomString();

        var found = Cached<string>.TryGet(k1, k2, k3, k4, k5, k6, k7, out var _);

        Assert.False(found);
    }

    [Fact]
    public void Cached_Saved_ReturnsTrueAndCorrectValue_K7()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var k7 = RandomString();
        var value = RandomString();

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
