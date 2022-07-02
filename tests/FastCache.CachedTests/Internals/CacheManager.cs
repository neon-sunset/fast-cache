using FastCache.Services;

namespace FastCache.CachedTests.Internals;

public sealed class CacheManagerTests
{
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.MinValue)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(-double.Epsilon)]
    [InlineData(100.00001)]
    [InlineData(100.1D)]
    [InlineData(double.MaxValue)]
    [InlineData(double.PositiveInfinity)]
    public void Trim_Throws_OnInvalidPercentage(double percentage)
    {
        void Trim() => CacheManager.Trim<int, object>(percentage);

        Assert.Throws<ArgumentOutOfRangeException>(Trim);
    }
}
