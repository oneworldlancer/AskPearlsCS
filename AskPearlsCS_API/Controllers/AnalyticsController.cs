using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/analytics")]
public sealed class AnalyticsController(IAnalytics_Service analytics, ICurrentUserContext_Service user) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int days = 30, [FromQuery] string? widget = null, [FromQuery] string? activityType = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(user.OwnerUserTokenID)) return Unauthorized();
        if (days is not (7 or 30 or 90)) return BadRequest(new { message = "Days must be 7, 30, or 90." });
        if (activityType is not null and not ("all" or "started" or "completed" or "recommended" or "published")) return BadRequest(new { message = "Invalid activity type." });
        return Ok(await analytics.GetAsync(user.OwnerUserTokenID, days, widget == "all" ? null : widget, activityType, ct));
    }
}
