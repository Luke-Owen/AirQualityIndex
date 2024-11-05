using System.Globalization;
using System.Web;
using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;
using AirQualityIndex.Models.OpenWeatherMap;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace AirQualityIndex.Services;

public class AirQualityService : IAirQualityService
{
    private readonly string? _baseUrl;
    private readonly string? _apiKey;
    private readonly HttpClient _httpClient;
    
    public AirQualityService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        var baseUrl = configuration.GetSection("OpenWeatherMapApiUrl").Value;
        var apiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }
    
    public async Task<List<AirQualityResponseModel>> GetAirQuality(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        uriBuilder.Path += "/history";

        var fromDateOffset = (DateTimeOffset)fromDate.ToUniversalTime();
        var toDateOffset = (DateTimeOffset)toDate.ToUniversalTime();
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lat"] = latitude.ToString(CultureInfo.InvariantCulture);
        query["lon"] = longitude.ToString(CultureInfo.InvariantCulture);
        query["start"] = fromDateOffset.ToUnixTimeSeconds().ToString();
        query["end"] =  toDateOffset.ToUnixTimeSeconds().ToString();
        query["appid"] = _apiKey;
        
        uriBuilder.Query = query.ToString();
        
        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        var resultContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get air qualities from OpenWeatherMap with code: {response.StatusCode} and content: {resultContent}");
        }
        
        var result = JsonConvert.DeserializeObject<AirQualityResponse>(resultContent);

        if (result?.List == null)
        {
            throw new JsonException($"Unable to deserialize air quality json. \n {resultContent}");
        }

        return result.List.Select(x => new AirQualityResponseModel
        {
            AirQualityIndex = x.Main.Aqi,
            Date = DateTimeOffset.FromUnixTimeSeconds(x.Dt).UtcDateTime
        }).ToList();
    }
    
    public async Task<AirQualityResponseModel> GetCurrentAirQuality(decimal latitude, decimal longitude)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lat"] = latitude.ToString(CultureInfo.InvariantCulture);
        query["lon"] = longitude.ToString(CultureInfo.InvariantCulture);
        query["appid"] = _apiKey;
        
        uriBuilder.Query = query.ToString();
        
        // make API Call with the HttpClient, will need to set it up.
        var response = await _httpClient.GetAsync(uriBuilder.ToString());
        var resultContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Unable to get air quality from OpenWeatherMap with code: {response.StatusCode} and content: {resultContent}");
        }
        
        var result = JsonConvert.DeserializeObject<AirQualityResponse>(resultContent);
        
        if (result?.List[0].Main is null)
            throw new JsonException($"Unable to deserialize air quality json. \n {resultContent}");
        
        return new AirQualityResponseModel
        {
            AirQualityIndex = result.List[0].Main.Aqi,
            Date = DateTime.UtcNow
        };
    }
    
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude) => 
        $"airqualityindex_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}_{latitude}_{longitude}";
}