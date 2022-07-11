namespace FastCache.Benchmarks;

[ShortRunJob]
public class QuickList
{
    private const int Parallelism = 2;
    private static readonly (object, long) Key = (42069, 1337);
    private const long Ticks = 32894323429532;

    [Benchmark]
    public void Add()
    {
        var quickList = new EvictionQuickList<(object, long), string>();
        var length = quickList.FreeSpace;

        Parallel.For(0, Parallelism, _ =>
        {
            var sliceLength = length / Parallelism;
            for (uint i = 0; i < sliceLength; i++)
            {
                quickList.Add(Key, Ticks);
            }
        });

        quickList.Dispose();
    }

    [Benchmark]
    public void OverWritingAdd()
    {
        var quickList = new EvictionQuickList<(object, long), string>();
        var length = quickList.FreeSpace;

        Parallel.For(0, Parallelism, _ =>
        {
            var sliceLength = length / Parallelism;
            for (uint i = 0; i < sliceLength; i++)
            {
                quickList.OverwritingAdd(Key, Ticks);
            }
        });

        quickList.Dispose();
    }
}
