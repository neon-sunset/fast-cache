namespace FastCache.Cached.Tests;

public sealed class Cached_Sanity
{
    [Fact]
    public void CachedDefaultConstructor_AlwaysThrows()
    {
        static void RunCtor()
        {
            _ = new Cached<string, string>();
        }

        Assert.Throws<InvalidOperationException>(RunCtor);
    }
}