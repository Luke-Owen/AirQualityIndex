using AirQualityIndex.Models;

namespace AirQualityIndex.Interfaces;

public interface IAirQualityService
{
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude);
    public Task<AirQuality> GetCurrentAirQuality(decimal latitude, decimal longitude);
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude);
    
}