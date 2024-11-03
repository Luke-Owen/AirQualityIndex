using AirQualityIndex.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityIndex.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class AirQualityController(IApiService apiService) : ControllerBase
{

    [HttpGet]
    public IActionResult GetAirQualityIndex([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, 
        [FromQuery] KeyValuePair<decimal, decimal> coordinates)
    {
        var airQuality = apiService.GetAirQuality(fromDate, toDate, coordinates);
        return Ok(airQuality);
    }

    [HttpGet("[action]")]
    public IActionResult GetAppStatus()
    {
        return Ok("App is running");
    }
}
