namespace CachedExample;

public static class GetOrCompute
{
    public static async ValueTask Run()
    {
        const string query = "Kyiv?format=3";

        // 'Cached.LastOrCompute': Either get last cached value or execute delegate to retrieve and store new value in the cache
        var weather = await Cached.LastOrCompute(query, FetchWeather);

        // Call 'LastOrCompute' for the second time. Since we already called it once with the parameter 'query',
        // the method immediately returns the value we received earlier. By using ValueTask<T> we ensure there are no allocations
        // Note: starting with .NET 7, passing a 'MethodGroup' as 'Func' parameter will no longer allocate
        var cachedWeather = await Cached.LastOrCompute(query, FetchWeather);

        // In fact, the second call has returned the same cached string instance we received earlier from 'FetchWeather'
        Debug.Assert(ReferenceEquals(weather, cachedWeather));

        Console.WriteLine(weather);
    }

    private static async Task<string> FetchWeather(string query)
    {
        var client = new HttpClient();
        return await client.GetStringAsync($"https://wttr.in/{query}");
    }
}