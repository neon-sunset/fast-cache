using System.Runtime.CompilerServices;
using FastCache.Collections;

namespace FastCache.CachedTests.Collections;

public sealed class CachedRangeTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveRange_InsertedArray_MatchesDataInCache(bool multithreaded)
    {
        var array = GenerateArray(multithreaded);
        CachedRange<string>.Save(array, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveRange_InsertedArrayPairs_MatchDataInCache(bool multithreaded)
    {
        var array = GenerateArray(multithreaded);
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveRange_InsertedList_MatchesDataInCache(bool multithreaded)
    {
        var list = GenerateList(multithreaded);
        CachedRange<string>.Save(list, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in list)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveRange_InsertedEnumerable_MatchesDataInCache(bool multithreaded)
    {
        var list = GenerateList(multithreaded);
        var enumerable = list.Select(kvp => kvp);
        CachedRange<string>.Save(enumerable, TimeSpan.MaxValue);

        foreach (var (key, expectedValue) in list)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(expectedValue, cached.Value);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RemoveRange_RemovedArray_NoLongerPresentInCache(bool multithreaded)
    {
        var array = GenerateArray(multithreaded);
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RemoveRange_RemovedEnumerable_NoLongerPresentInCache(bool multithreaded)
    {
        var array = GenerateArray(multithreaded);
        var keys = array.Select(kvp => kvp.Key).ToArray();
        var enumerable = keys.Select(key => key).ToList();

        CachedRange<string>.Save(array, TimeSpan.MaxValue);
        foreach (var (key, value) in array)
        {
            var found = Cached<string>.TryGet(key, out var cached);

            Assert.True(found);
            Assert.Equal(value, cached.Value);
        }

        CachedRange<string>.Remove(enumerable);
        foreach (var key in keys)
        {
            var found = Cached<string>.TryGet(key, out _);

            Assert.False(found);
        }
    }

    private static (string Key, string Value)[] GenerateArray(bool lengthAboveMtThreshold, [CallerMemberName] string key = "")
    {
        var length = lengthAboveMtThreshold
            ? (int)(Constants.ParallelSaveMinBatchSize * Environment.ProcessorCount) + 7
            : (int)Constants.ParallelSaveMinBatchSize;

        var array = new (string, string)[length];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (GetTestKey(key, i, lengthAboveMtThreshold), GetRandomString());
        }

        return array;
    }

    private static List<(string Key, string Value)> GenerateList(bool lengthAboveMtThreshold, [CallerMemberName] string key = "")
    {
        var length = lengthAboveMtThreshold
            ? (int)(Constants.ParallelSaveMinBatchSize * Environment.ProcessorCount) + 7
            : (int)Constants.ParallelSaveMinBatchSize;

        var list = new List<(string, string)>(length);

        for (int i = 0; i < list.Capacity; i++)
        {
            list.Add((GetTestKey(key, i, lengthAboveMtThreshold), GetRandomString()));
        }

        return list;
    }
}
