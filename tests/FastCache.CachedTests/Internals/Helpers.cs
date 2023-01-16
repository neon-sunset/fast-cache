using FastCache.Helpers;

namespace FastCache.CachedTests;

public sealed class Helpers
{
    // [Fact] - Currently callbacks are not triggered because testing framework suppresses GC
    // public void Gen2GCCallback_IsTriggerdOnGC()
    // {
    //     var triggered = false;
    //     Gen2GcCallback.Register(() =>
    //     {
    //         ref var flag = ref triggered;

    //         flag = true;
    //     });

    //     GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

    //     Assert.True(triggered);
    // }

    [Fact]
    public void TypeInfo_IsManaged_ReturnsCorrectValues()
    {
#if !NETSTANDARD2_0 && !NET48
        Assert.True(TypeInfo.IsManaged<(string, long)>());
#endif 
        Assert.True(TypeInfo.IsManaged<object>());

        Assert.False(TypeInfo.IsManaged<(int, long)>());
        Assert.False(TypeInfo.IsManaged<int>());
    }
}
