namespace FastCache.CachedTests;

public sealed class CachedTests_InputValidation
{
    public static IEnumerable<object[]> ValidExpirations()
    {
        yield return new object[] { TimeSpan.FromTicks(1) };
        yield return new object[] { TimeSpan.FromMilliseconds(1) };
        yield return new object[] { TimeSpan.FromSeconds(1) };
        yield return new object[] { TimeSpan.FromMinutes(1) };
        yield return new object[] { TimeSpan.FromHours(1) };
        yield return new object[] { TimeSpan.FromDays(1) };
        yield return new object[] { TimeSpan.MaxValue };
    }

    public static IEnumerable<object[]> InvalidExpirations()
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
        void SaveInvalidExpiration()
        {
            if (!Cached<string>.TryGet(GetTestKey(expiration), out var cached))
            {
                cached.Save(GetRandomString(), expiration);
            }
        }

        Assert.Throws<ArgumentOutOfRangeException>(SaveInvalidExpiration);
    }

    [Theory]
    [MemberData(nameof(ValidExpirations))]
    public void Cached_Save_DoesNotThrowOnValidExpiration(TimeSpan expiration)
    {
        Cached<string>.TryGet(GetTestKey(expiration), out var cached);
        cached.Save(GetRandomString(), expiration);
    }
}
