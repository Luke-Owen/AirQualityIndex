using System.Globalization;
using System.Web;
using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace AirQualityIndex.Services;

public class AirQualityService : IAirQualityService
{
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    
    public AirQualityService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        var baseUrl = configuration.GetSection("OpenWeatherMapApiUrl").Value;
        var apiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
        _baseUrl = baseUrl ?? throw new NullReferenceException("The OpenWeatherMapApiUrl configuration is missing");
        _apiKey = apiKey ?? throw new NullReferenceException("The OpenWeatherMapApiKey is missing");
    }
    
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude)
    {
        // use async methods in the http client
        return Task.FromResult(new List<AirQuality>());
    }
    
    public async Task<AirQuality> GetCurrentAirQuality(decimal latitude, decimal longitude)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lat"] = latitude.ToString(CultureInfo.InvariantCulture);
        query["lon"] = longitude.ToString(CultureInfo.InvariantCulture);
        query["appid"] = _apiKey;
        
        uriBuilder.Query = query.ToString();
        
        // make API Call with the HttpClient, will need to set it up.
        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Unable to get air quality. Status Code: {response.StatusCode}");
        
        var resultContent = await response.Content.ReadAsStringAsync();
        
        var result = JsonConvert.DeserializeObject<AirQualityResponse>(resultContent);
        
        if (result?.List[0].Main is null)
            throw new JsonException($"Unable to deserialize air quality json. \n {response.Content}");
        
        return result.List[0].Main;
    }
    
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude) => 
        $"airqualityindex_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}_{latitude}_{longitude}";
}