namespace FastCache.CachedTests;

public sealed class CachedTests_GetOrCompute
{
    [Fact]
    public void GetOrComputeK1_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k1 => Delegate(k1), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k1 => Delegate(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK1Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k1 => Delegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k1 => Delegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK2_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, (k1, k2) => Delegate((k1, k2)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, (k1, k2) => Delegate((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK2Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, (k1, k2) => Delegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, (k1, k2) => Delegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK3_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => Delegate((k1, k2, k3)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => Delegate((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK3Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => Delegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => Delegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK4_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => Delegate((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => Delegate((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK4Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => Delegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => Delegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK5_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => Delegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => Delegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK5Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => Delegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => Delegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK6_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => Delegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => Delegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK6Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => Delegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => Delegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK7_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => Delegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => Delegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public void GetOrComputeK7Limit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => Delegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => Delegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK1ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k1 => ValueTaskDelegate(k1), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => ValueTaskDelegate(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK1ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k1 => ValueTaskDelegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => ValueTaskDelegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK2ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => ValueTaskDelegate((k1, k2)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => ValueTaskDelegate((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK2ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => ValueTaskDelegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => ValueTaskDelegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK3ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => ValueTaskDelegate((k1, k2, k3)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => ValueTaskDelegate((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK3ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => ValueTaskDelegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => ValueTaskDelegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK4ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => ValueTaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => ValueTaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK4ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => ValueTaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => ValueTaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK5ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => ValueTaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => ValueTaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK5ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => ValueTaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => ValueTaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK6ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK6ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK7ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK7ValueTaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => ValueTaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK1Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k1 => TaskDelegate(k1), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => TaskDelegate(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK1TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k1 => TaskDelegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => TaskDelegate(k1), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK2Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => TaskDelegate((k1, k2)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => TaskDelegate((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK2TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => TaskDelegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => TaskDelegate((k1, k2)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK3Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => TaskDelegate((k1, k2, k3)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => TaskDelegate((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK3TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => TaskDelegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => TaskDelegate((k1, k2, k3)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK4Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => TaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => TaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK4TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => TaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => TaskDelegate((k1, k2, k3, k4)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK5Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => TaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => TaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK5TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => TaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => TaskDelegate((k1, k2, k3, k4, k5)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK6Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => TaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => TaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK6TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => TaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => TaskDelegate((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK7Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => TaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => TaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    [Fact]
    public async Task GetOrComputeK7TaskLimit_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = GetTestKey();
        var k2 = GetTestKey();
        var k3 = GetTestKey();
        var k4 = GetTestKey();
        var k5 = GetTestKey();
        var k6 = GetTestKey();
        var k7 = GetTestKey();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => TaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => TaskDelegate((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue, limit: int.MaxValue);

        Assert.Equal(before, after);
        Assert.True(ReferenceEquals(before, after));
    }

    private static string Delegate<K>(K input)
    {
        return input + GetRandomString();
    }

    private static async ValueTask<string> ValueTaskDelegate<K>(K input)
    {
        await Task.Yield();

        return input + GetRandomString();
    }

    private static Task<string> TaskDelegate<K>(K input)
    {
        return Task.FromResult(input + GetRandomString());
    }
}