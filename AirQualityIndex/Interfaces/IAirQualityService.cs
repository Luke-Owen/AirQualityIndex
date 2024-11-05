using AirQualityIndex.Models;

namespace AirQualityIndex.Interfaces;

public interface IAirQualityService
{
    public Task<List<AirQualityResponseModel>> GetAirQuality(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude);
    public Task<AirQualityResponseModel> GetCurrentAirQuality(decimal latitude, decimal longitude);
    public string AirQualityIndexKey(DateTime fromDate, DateTime toDate, decimal latitude, decimal longitude);
    
}