using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AirQualityIndex.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController(HealthCheckService healthCheckService) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetHealth()
    {
        var healthReport = await healthCheckService.CheckHealthAsync();
        return healthReport.Status == HealthStatus.Healthy 
            ? Ok("Healthy") 
            : StatusCode(503, "Unhealthy");
    }
}
