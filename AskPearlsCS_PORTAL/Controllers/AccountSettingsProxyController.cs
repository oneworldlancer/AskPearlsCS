using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Account/Api")]
public sealed class AccountSettingsProxyController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly ICurrentUserContext_Service _currentUser;
    private readonly IApiJwtToken_Service _jwt;

    public AccountSettingsProxyController(
        IHttpClientFactory clients, IConfiguration configuration,
        IAntiforgery antiforgery, ICurrentUserContext_Service currentUser,
        IApiJwtToken_Service jwt)
    {
        _clients = clients;
        _configuration = configuration;
        _antiforgery = antiforgery;
        _currentUser = currentUser;
        _jwt = jwt;
    }

    [HttpGet("settings")]
    public Task<IActionResult> GetSettings(CancellationToken ct) => Forward(HttpMethod.Get, ct);

    [HttpPut("settings")]
    public Task<IActionResult> SaveSettings(CancellationToken ct) => Forward(HttpMethod.Put, ct);

    private async Task<IActionResult> Forward(HttpMethod method, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Unauthorized(new { success = false, message = "Sign in to continue." });

        if (method != HttpMethod.Get)
        {
            try { await _antiforgery.ValidateRequestAsync(HttpContext); }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new { success = false, message = "Invalid anti-forgery token." });
            }
        }

        var baseUrl = _configuration["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var origin) ||
            origin.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(origin.Query) || !string.IsNullOrEmpty(origin.Fragment) ||
            origin.AbsolutePath != "/")
            return Problem(statusCode: 500, title: "Invalid AskPearlsApi:BaseUrl configuration.");

        string token;
        try { token = _jwt.CreateForCurrentUser(); }
        catch (InvalidOperationException)
        {
            return Problem(statusCode: 500, title: "API JWT configuration is invalid.");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, message = "Invalid authenticated user." });
        }

        using var request = new HttpRequestMessage(method, new Uri(origin, "api/account/settings"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (method == HttpMethod.Put)
        {
            request.Content = new StreamContent(Request.Body);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        try
        {
            var client = _clients.CreateClient();
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json; charset=utf-8",
                Content = body
            };
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Problem(statusCode: 504, title: "AskPearls API timed out.");
        }
        catch (HttpRequestException)
        {
            return Problem(statusCode: 502, title: "Could not reach AskPearls API.");
        }
    }
}
