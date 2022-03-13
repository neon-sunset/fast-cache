namespace CachedExample;

public static class GetOrCompute
{
    public static async ValueTask Run()
    {
        const string query = "Kyiv?format=3";
        var oneSecond = TimeSpan.FromSeconds(1);

        // 'Cached.LastOrCompute': Either get last cached value or execute delegate to retrieve and store new value in the cache
        var weather = await Cached.GetOrCompute(query, FetchWeather, oneSecond);
        Console.WriteLine($"New: {weather}");

        // Call 'LastOrCompute' for the second time. Since we already called it once with the parameter 'query',
        // the method immediately returns the value we received earlier. By using ValueTask<T> we ensure there are no allocations
        // Note: starting with .NET 7, passing a 'MethodGroup' as 'Func' parameter will no longer allocate
        var cachedWeather = await Cached.GetOrCompute(query, FetchWeather, oneSecond);
        Console.WriteLine($"Cached: {cachedWeather}");

        // In fact, the second call has returned the same cached string instance we received earlier from 'FetchWeather'
        Debug.Assert(ReferenceEquals(weather, cachedWeather));

        // Wait for one second
        await Task.Delay(oneSecond);

        // Retrieve weather once again because previously cached value has expired
        weather = await Cached.GetOrCompute(query, FetchWeather, oneSecond);
        Console.WriteLine($"New: {weather}");
    }

    private static async Task<string> FetchWeather(string query)
    {
        Console.WriteLine($"Fetching weather for {query}");
        var client = new HttpClient();
        return await client.GetStringAsync($"https://wttr.in/{query}");
    }
}
