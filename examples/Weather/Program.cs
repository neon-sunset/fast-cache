using FastCache;

var query = "Kyiv?format=3";
var http = new HttpClient();

Task<string> FetchWeather(string query)
{
    var uri = new Uri($"https://wttr.in/{query}");
    Console.WriteLine($"Requesting weather from {uri}!");

    return http.GetStringAsync(uri);
}

Console.WriteLine(
    "'Cached.GetOrCompute': Get 'T' value from the cache or execute passed delegate.'\n" +
    "If computed, the value will expire in 'expiration' time you have passed. In this case it's 5 seconds.\n");

var value = await Cached.GetOrCompute(query, FetchWeather, TimeSpan.FromSeconds(5));

Console.WriteLine($"Weather: {value}");

Console.WriteLine(
    "Call 'GetOrCompute' for the second time.\n" +
    "Since we just invoked it, the method immediately returns previously cached weather.\n" +
    "Note: 'GetOrCompute' always returns 'ValueTask<T>' instead of 'Task' to avoid allocations when returning cached value.\n");

var cachedValue = await Cached.GetOrCompute(query, FetchWeather, TimeSpan.FromSeconds(5));

Console.WriteLine($"Cached weather: {cachedValue}");

Console.WriteLine(
    "In fact, the second call has returned the same cached string instance we initially received from 'FetchWeather'\n" +
    $"Is 'value' reference same as 'cachedValue': {ReferenceEquals(value, cachedValue)}\n");

Console.WriteLine("Let's wait for five seconds\n");
await Task.Delay(TimeSpan.FromSeconds(5));

Console.WriteLine(
    "Retrieve weather once again because the value we cached has expired.\n" +
    "There is no need to worry that we run out of memory because 'FastCache.Cached' will automatically evict expired values.\n" +
    "By default, quick and full eviction will run as often as 15 and .\n");

value = await Cached.GetOrCompute(query, FetchWeather, TimeSpan.FromSeconds(5));

Console.WriteLine($"Weather: {value}");
Console.WriteLine("Done!");
Console.ReadLine();
