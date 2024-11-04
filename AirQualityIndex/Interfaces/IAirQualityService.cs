using AirQualityIndex.Models;

namespace AirQualityIndex.Interfaces;

public interface IAirQualityService
{
    public Task<List<AirQuality>> GetAirQuality(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates);
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates);
}