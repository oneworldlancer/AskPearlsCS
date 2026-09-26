using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        product = "AskPearls DecisionOS",
        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
        utc = DateTime.UtcNow
    });
}
