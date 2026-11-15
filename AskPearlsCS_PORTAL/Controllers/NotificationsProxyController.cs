using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Account/Api/notifications")]
public sealed class NotificationsProxyController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _config;
    private readonly IAntiforgery _antiforgery;
    private readonly ICurrentUserContext_Service _user;
    private readonly IApiJwtToken_Service _jwt;
    public NotificationsProxyController(IHttpClientFactory clients, IConfiguration config, IAntiforgery antiforgery,
        ICurrentUserContext_Service user, IApiJwtToken_Service jwt)
    { _clients = clients; _config = config; _antiforgery = antiforgery; _user = user; _jwt = jwt; }

    [HttpGet("")]
    public Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 30, CancellationToken ct = default) =>
        Forward(HttpMethod.Get, $"?skip={Math.Max(0, skip)}&take={Math.Clamp(take, 1, 100)}", ct);
    [HttpGet("unread-count")]
    public Task<IActionResult> Count(CancellationToken ct) => Forward(HttpMethod.Get, "unread-count", ct);
    [HttpPut("{id:long}/read")]
    public Task<IActionResult> Read(long id, CancellationToken ct) => Forward(HttpMethod.Put, $"{id}/read", ct);
    [HttpPut("read-all")]
    public Task<IActionResult> ReadAll(CancellationToken ct) => Forward(HttpMethod.Put, "read-all", ct);
    [HttpGet("preferences")]
    public Task<IActionResult> Preferences(CancellationToken ct) => Forward(HttpMethod.Get, "preferences", ct);
    [HttpPut("preferences")]
    public Task<IActionResult> SavePreferences(CancellationToken ct) => Forward(HttpMethod.Put, "preferences", ct);

    private async Task<IActionResult> Forward(HttpMethod method, string path, CancellationToken ct)
    {
        if (!_user.IsAuthenticated || string.IsNullOrWhiteSpace(_user.OwnerUserTokenID)) return Unauthorized();
        if (method != HttpMethod.Get)
        {
            try { await _antiforgery.ValidateRequestAsync(HttpContext); }
            catch (AntiforgeryValidationException) { return BadRequest(new { message = "Invalid anti-forgery token." }); }
        }
        var baseUrl = _config["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var origin) || origin.Scheme != Uri.UriSchemeHttps ||
            origin.AbsolutePath != "/" || !string.IsNullOrEmpty(origin.Query) || !string.IsNullOrEmpty(origin.Fragment))
            return Problem(statusCode: 500, title: "Invalid AskPearlsApi:BaseUrl configuration.");
        string token;
        try { token = _jwt.CreateForCurrentUser(); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
        catch (InvalidOperationException) { return Problem(statusCode: 500, title: "API JWT configuration invalid."); }
        var relative = "api/notifications/" + path;
        using var request = new HttpRequestMessage(method, new Uri(origin, relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (method == HttpMethod.Put && path == "preferences")
        {
            request.Content = new StreamContent(Request.Body);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
        try
        {
            using var response = await _clients.CreateClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                Content = await response.Content.ReadAsStringAsync(ct)
            };
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return Problem(statusCode: 504, title: "API timed out."); }
        catch (HttpRequestException) { return Problem(statusCode: 502, title: "API unavailable."); }
    }
}
