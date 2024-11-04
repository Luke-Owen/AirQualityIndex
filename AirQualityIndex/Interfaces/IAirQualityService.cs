using AirQualityIndex.Models;

namespace AirQualityIndex.Interfaces;

public interface IAirQualityService
{
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, string latitude, string longitude);
    public Task<AirQuality> GetCurrentAirQuality(string latitude, string longitude);
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, string latitude, string longitude);
    
}