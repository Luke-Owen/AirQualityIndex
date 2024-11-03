using AirQualityIndex.Models;

namespace AirQualityIndex.Interfaces;

public interface IApiService
{
    public List<AirQuality> GetAirQuality(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates);
}