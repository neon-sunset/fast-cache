using FastCache.Collections;

namespace FastCache.Cached.Tests.Collections;

public sealed class CachedRangeTests
{
    [Fact]
    public void SaveRange_InsertedArray_MatchesDataInCache()
    {
        var array = GenerateArray();
        CachedRange.Save(array, TimeSpan.FromHours(1));

        foreach (var (key, expectedValue) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Fact]
    public void SaveRange_InsertedList_MatchesDataInCache()
    {
        var list = GenerateList();
        CachedRange.Save(list, TimeSpan.FromHours(1));

        foreach (var (key, expectedValue) in list)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Fact]
    public void SaveRange_InsertedEnumerable_MatchesDataInCache()
    {
        var list = GenerateList();
        var enumerable = list.Select(kvp => kvp);
        CachedRange.Save(enumerable, TimeSpan.FromHours(1));

        foreach (var (key, expectedValue) in list)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    private static (long, string)[] GenerateArray()
    {
        var sequence = new (long, string)[8192];

        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = (RandomLong(), RandomString());
        }

        return sequence;
    }

    private static List<(long, string)> GenerateList()
    {
        var list = new List<(long, string)>(8192);

        for (int i = 0; i < list.Capacity; i++)
        {
            list.Add((RandomLong(), RandomString()));
        }

        return list;
    }
}
