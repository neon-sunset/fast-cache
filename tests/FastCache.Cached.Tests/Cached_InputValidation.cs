namespace FastCache.Cached.Tests;

public sealed class Cached_InputValidation
{
    private static IEnumerable<object[]> ValidExpirations()
    {
        yield return new object[] { TimeSpan.FromTicks(1) };
        yield return new object[] { TimeSpan.FromMilliseconds(1) };
        yield return new object[] { TimeSpan.FromSeconds(1) };
        yield return new object[] { TimeSpan.FromMinutes(1) };
        yield return new object[] { TimeSpan.FromHours(1) };
        yield return new object[] { TimeSpan.FromDays(1) };
        yield return new object[] { TimeSpan.MaxValue };
    }

    private static IEnumerable<object[]> InvalidExpirations()
    {
        yield return new object[] { TimeSpan.Zero };
        yield return new object[] { TimeSpan.MinValue };
        yield return new object[] { TimeSpan.FromTicks(-1) };
        yield return new object[] { TimeSpan.FromMilliseconds(-1) };
        yield return new object[] { TimeSpan.FromSeconds(-1) };
        yield return new object[] { TimeSpan.FromMinutes(-1) };
        yield return new object[] { TimeSpan.FromHours(-1) };
        yield return new object[] { TimeSpan.FromDays(-1) };
    }

    [Fact]
    public void CachedDefaultConstructor_AlwaysThrows()
    {
        static void RunCtor()
        {
            _ = new Cached<string, string>();
        }

        Assert.Throws<InvalidOperationException>(RunCtor);
    }

    [Theory]
    [MemberData(nameof(InvalidExpirations))]
    public void Cached_Save_ThrowsOnInvalidExpiration(TimeSpan expiration)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            if (!Cached<string>.TryGet(RandomString(), out var cached))
            {
                cached.Save(RandomString(), expiration);
            }
        });
    }

    [Theory]
    [MemberData(nameof(ValidExpirations))]
    public void Cached_Save_DoesNotThrowOnValidExpiration(TimeSpan expiration)
    {
        Cached<string>.TryGet(RandomString(), out var cached);
        cached.Save(RandomString(), expiration);
    }
}
