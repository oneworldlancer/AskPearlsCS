using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Account/Api/subscription")]
public sealed class SubscriptionProxyController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly ICurrentUserContext_Service _current;
    private readonly IApiJwtToken_Service _jwt;

    public SubscriptionProxyController(IHttpClientFactory clients, IConfiguration configuration,
        IAntiforgery antiforgery, ICurrentUserContext_Service current, IApiJwtToken_Service jwt)
    {
        _clients = clients; _configuration = configuration; _antiforgery = antiforgery;
        _current = current; _jwt = jwt;
    }

    [HttpGet("plans")]
    public Task<IActionResult> Plans(CancellationToken ct) => Forward(HttpMethod.Get, "plans", ct);
    [HttpGet("current")]
    public Task<IActionResult> Current(CancellationToken ct) => Forward(HttpMethod.Get, "current", ct);
    [HttpGet("history")]
    public Task<IActionResult> History(CancellationToken ct) => Forward(HttpMethod.Get, "history", ct);
    [HttpPost("ensure-free")]
    public Task<IActionResult> EnsureFree(CancellationToken ct) => Forward(HttpMethod.Post, "ensure-free", ct);
    [HttpPost("checkout")]
    public Task<IActionResult> Checkout(CancellationToken ct) => Forward(HttpMethod.Post, "checkout", ct);

    private async Task<IActionResult> Forward(HttpMethod method, string path, CancellationToken ct)
    {
        if (!_current.IsAuthenticated || string.IsNullOrWhiteSpace(_current.OwnerUserTokenID))
            return Unauthorized(new { success = false, message = "Sign in to continue." });
        if (method != HttpMethod.Get)
        {
            try { await _antiforgery.ValidateRequestAsync(HttpContext); }
            catch (AntiforgeryValidationException)
            { return BadRequest(new { success = false, message = "Invalid anti-forgery token." }); }
        }
        var baseUrl = _configuration["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var origin) ||
            origin.Scheme != Uri.UriSchemeHttps || origin.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(origin.Query) || !string.IsNullOrEmpty(origin.Fragment))
            return Problem(statusCode: 500, title: "Invalid AskPearlsApi:BaseUrl configuration.");
        string jwt;
        try { jwt = _jwt.CreateForCurrentUser(); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
        catch (InvalidOperationException)
        { return Problem(statusCode: 500, title: "API JWT configuration is invalid."); }
        using var request = new HttpRequestMessage(method, new Uri(origin, "api/account/subscription/" + path));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        try
        {
            using var response = await _clients.CreateClient().SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, ct);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                Content = await response.Content.ReadAsStringAsync(ct)
            };
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { return Problem(statusCode: 504, title: "AskPearls API timed out."); }
        catch (HttpRequestException)
        { return Problem(statusCode: 502, title: "AskPearls API unavailable."); }
    }
}
