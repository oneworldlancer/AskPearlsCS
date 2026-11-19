using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Analytics/Api")]
public sealed class AnalyticsProxyController(IHttpClientFactory clients, IConfiguration config, ICurrentUserContext_Service current, IApiJwtToken_Service jwt) : Controller
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] int days = 30, [FromQuery] string? widget = null, [FromQuery] string? activityType = null, CancellationToken ct = default)
    {
        if (!current.IsAuthenticated || string.IsNullOrWhiteSpace(current.OwnerUserTokenID)) return Unauthorized();
        if (days is not (7 or 30 or 90)) return BadRequest();
        var origin = config["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme != "https") return Problem(statusCode: 500, title: "Invalid API base URL.");
        var qs = "?days=" + days + (string.IsNullOrWhiteSpace(widget) ? "" : "&widget=" + Uri.EscapeDataString(widget)) + (string.IsNullOrWhiteSpace(activityType) ? "" : "&activityType=" + Uri.EscapeDataString(activityType));
        using var req = new HttpRequestMessage(HttpMethod.Get, new Uri(uri, "/api/analytics" + qs));
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.CreateForCurrentUser());
        try { using var res = await clients.CreateClient("AskPearlsWidgetApi").SendAsync(req, ct); return new ContentResult { StatusCode = (int)res.StatusCode, ContentType = "application/json", Content = await res.Content.ReadAsStringAsync(ct) }; }
        catch (HttpRequestException) { return Problem(statusCode: 502, title: "Analytics API unavailable."); }
    }
}
