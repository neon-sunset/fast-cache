namespace FastCache.Cached.Tests;

public sealed class Cached_Sanity
{
    [Fact]
    public void CachedDefaultConstructor_AlwaysThrows()
    {
        static void RunCtor()
        {
            new Cached<string, string>();
        }

        Assert.Throws<InvalidOperationException>(RunCtor);
    }
}