using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;
using AirQualityIndex.Models.QueryParameters;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityIndex.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class AirQualityController(IAirQualityService airQualityService, IRedisService redisService) : ControllerBase
{

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAirQualityIndex([FromQuery] AirQualityIndexQueryParams queryParams)
    {
        var airQualityIndexKey =
            airQualityService.AirQualityIndexKey(queryParams.FromDate, queryParams.ToDate, queryParams.Coordinates);

        var airQuality = await redisService.GetAsync<List<AirQuality>?>(airQualityIndexKey);

        if (airQuality != null) 
            return Ok(airQuality);
        
        airQuality = await airQualityService.GetAirQuality(queryParams.FromDate, queryParams.ToDate, queryParams.Coordinates);
        await redisService.SetAsync(airQualityIndexKey, airQuality, TimeSpan.FromDays(1));

        return Ok(airQuality);
    }

    [HttpGet("[action]")]
    public IActionResult GetAppStatus()
    {
        return Ok("App is running");
    }
}
