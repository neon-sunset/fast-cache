using FastCache.Collections;

namespace FastCache.Cached.Tests.Collections;

public sealed class CachedRangeTests
{
    [Fact]
    public void SaveRange_InsertedArray_MatchesDataInCache()
    {
        var array = GenerateArray();
        CachedRange<string>.Save(array, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Fact]
    public void SaveRange_InsertedArrayPairs_MatchDataInCache()
    {
        var array = GenerateArray();
        var keys = array.Select(kvp => kvp.Key).ToArray();
        var values = array.Select(kvp => kvp.Value).ToArray();
        CachedRange<string>.Save(keys, values, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Fact]
    public void SaveRange_InsertArrayPairs_Throw_OnLengthMismatch()
    {
        static void SaveMismatched()
        {
            var keys = Enumerable.Range(0, 64).ToArray();
            var values = Enumerable.Range(0, 32).Select(v => v.ToString()).ToArray();
            CachedRange<string>.Save(keys, values, TimeSpan.MaxValue);
        }

        Assert.Throws<ArgumentOutOfRangeException>(SaveMismatched);
    }

    [Fact]
    public void SaveRange_InsertedList_MatchesDataInCache()
    {
        var list = GenerateList();
        CachedRange<string>.Save(list, TimeSpan.MaxValue);

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
        CachedRange<string>.Save(enumerable, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in list)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Fact]
    public void RemoveRange_RemovedArray_NoLongerPresentInCache()
    {
        var array = GenerateArray();
        var keys = array.Select(kvp => kvp.Key).ToArray();

        CachedRange<string>.Save(array, TimeSpan.MaxValue);
        foreach (var (key, value) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(value, cached.Value);
        }

        CachedRange<string>.Remove(keys);
        foreach (var key in keys)
        {
            var found = Cached<string>.TryGet(key, out _);

            Assert.False(found);
        }
    }

    private static (long Key, string Value)[] GenerateArray()
    {
        var sequence = new (long, string)[8192];

        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = (RandomLong(), RandomString());
        }

        return sequence;
    }

    private static List<(long Key, string Value)> GenerateList()
    {
        var list = new List<(long, string)>(8192);

        for (int i = 0; i < list.Capacity; i++)
        {
            list.Add((RandomLong(), RandomString()));
        }

        return list;
    }
}
