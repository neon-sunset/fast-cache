namespace FastCache.CachedTests;

public sealed class CachedTests_GetOrCompute
{
    [Fact]
    public void GetOrComputeK1_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var before = Cached.GetOrCompute(k1, k1 => GetRandomString(k1), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k1 => GetRandomString(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK2_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomString((k1, k2)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomString((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK3_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomString((k1, k2, k3)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomString((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK4_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomString((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomString((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK5_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomString((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomString((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK6_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomString((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomString((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public void GetOrComputeK7_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var k7 = RandomString();
        var before = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomString((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomString((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK1ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var before = await Cached.GetOrCompute(k1, k1 => GetRandomStringValueTask(k1), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => GetRandomStringValueTask(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK2ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomStringValueTask((k1, k2)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomStringValueTask((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK3ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomStringValueTask((k1, k2, k3)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomStringValueTask((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK4ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomStringValueTask((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomStringValueTask((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK5ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomStringValueTask((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomStringValueTask((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK6ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomStringValueTask((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomStringValueTask((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK7ValueTask_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var k7 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomStringValueTask((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomStringValueTask((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK1Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var before = await Cached.GetOrCompute(k1, k1 => GetRandomStringTask(k1), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k1 => GetRandomStringTask(k1), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK2Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomStringTask((k1, k2)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, (k1, k2) => GetRandomStringTask((k1, k2)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK3Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomStringTask((k1, k2, k3)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, (k1, k2, k3) => GetRandomStringTask((k1, k2, k3)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK4Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomStringTask((k1, k2, k3, k4)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, (k1, k2, k3, k4) => GetRandomStringTask((k1, k2, k3, k4)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK5Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomStringTask((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, (k1, k2, k3, k4, k5) => GetRandomStringTask((k1, k2, k3, k4, k5)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK6Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomStringTask((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, (k1, k2, k3, k4, k5, k6) => GetRandomStringTask((k1, k2, k3, k4, k5, k6)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    [Fact]
    public async Task GetOrComputeK7Task_CachesResult_And_CallsDelegateExactlyOnce()
    {
        var k1 = RandomString();
        var k2 = RandomString();
        var k3 = RandomString();
        var k4 = RandomString();
        var k5 = RandomString();
        var k6 = RandomString();
        var k7 = RandomString();
        var before = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomStringTask((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);
        var after = await Cached.GetOrCompute(k1, k2, k3, k4, k5, k6, k7, (k1, k2, k3, k4, k5, k6, k7) => GetRandomStringTask((k1, k2, k3, k4, k5, k6, k7)), TimeSpan.MaxValue);

        Assert.Equal(before, after);
    }

    private static string GetRandomString<K>(K input)
    {
        return input + RandomString();
    }

        private static ValueTask<string> GetRandomStringValueTask<K>(K input)
    {
        return ValueTask.FromResult(input + RandomString());
    }

    private static Task<string> GetRandomStringTask<K>(K input)
    {
        return Task.FromResult(input + RandomString());
    }
}