using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;

namespace AirQualityIndex.Services;

public class ApiService : IApiService
{
    public List<AirQuality> GetAirQuality(DateTime fromDate, DateTime toDate, KeyValuePair<decimal, decimal> coordinates)
    {
        return new List<AirQuality>();
    }
}