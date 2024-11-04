using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;

namespace AirQualityIndex.Services;

public class AirQualityService : IAirQualityService
{
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates) => 
        $"airqualityindex_{fromDate:yyyy-MM-dd}_{toDate:yyyy-MM-dd}_{coordinates.Key}_{coordinates.Value}";
    
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates)
    {
        // use async methods in the http client
        return Task.FromResult(new List<AirQuality>());
    }
}