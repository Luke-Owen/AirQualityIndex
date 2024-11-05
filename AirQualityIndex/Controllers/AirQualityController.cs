using AirQualityIndex.Interfaces;
using AirQualityIndex.Models;
using AirQualityIndex.Models.QueryParameters;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityIndex.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class AirQualityController(IAirQualityService airQualityService, IRedisService redisService) : ControllerBase
{
    [ProducesResponseType<List<AirQuality>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAirQualityIndex([FromQuery] AirQualityIndexQueryParams queryParams)
    {
        var airQualityIndexKey =
            airQualityService.AirQualityIndexKey(queryParams.FromDate, queryParams.ToDate, queryParams.Latitude, queryParams.Longitude);

        var airQuality = await redisService.GetAsync<List<AirQuality>?>(airQualityIndexKey);

        if (airQuality != null) 
            return Ok(airQuality);
        
        airQuality = await airQualityService.GetAirQuality(queryParams.FromDate, queryParams.ToDate, queryParams.Latitude, queryParams.Longitude);
        await redisService.SetAsync(airQualityIndexKey, airQuality, TimeSpan.FromDays(1));

        return Ok(airQuality);
    }

    [ProducesResponseType<AirQuality>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetCurrentAirQualityIndex([FromQuery] CoordinatesQueryParams queryParams)
    {
        var airQuality = await airQualityService.GetCurrentAirQuality(queryParams.Latitude, queryParams.Longitude);
        return Ok(airQuality);
    }
}
