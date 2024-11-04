using System.Web;
using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;

namespace AirQualityIndex.Services;

public class AirQualityService : IAirQualityService
{
    private readonly string _baseUrl;
    private readonly string _apiKey;
    
    public AirQualityService(IConfiguration configuration)
    {
        var baseUrl = configuration.GetSection("OpenWeatherMapApiUrl").Value;
        var apiKey = configuration.GetSection("OpenWeatherMapApiKey").Value;
        _baseUrl = baseUrl ?? throw new NullReferenceException("The OpenWeatherMapApiUrl configuration is missing");
        _apiKey = apiKey ?? throw new NullReferenceException("The OpenWeatherMapApiKey is missing");
    }
    
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, string latitude, string longitude)
    {
        // use async methods in the http client
        return Task.FromResult(new List<AirQuality>());
    }
    
    public Task<AirQuality> GetCurrentAirQuality(string latitude, string longitude)
    {
        var uriBuilder = new UriBuilder(_baseUrl);
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["lat"] = latitude;
        query["lon"] = longitude;
        query["appid"] = _apiKey;
        
        uriBuilder.Query = query.ToString();
        
        // make API Call with the HttpClient, will need to set it up.
        
        return Task.FromResult(new AirQuality());
    }
    
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, string latitude, string longitude) => 
        $"airqualityindex_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}_{latitude}_{longitude}";
}