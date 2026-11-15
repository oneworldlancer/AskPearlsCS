using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Account/Api/companies")]
public sealed class CompaniesProxyController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly ICurrentUserContext_Service _current;
    private readonly IApiJwtToken_Service _jwt;
    public CompaniesProxyController(IHttpClientFactory clients, IConfiguration configuration,
        IAntiforgery antiforgery, ICurrentUserContext_Service current, IApiJwtToken_Service jwt)
    { _clients = clients; _configuration = configuration; _antiforgery = antiforgery; _current = current; _jwt = jwt; }

    [HttpGet("organizations")]
    public Task<IActionResult> Organizations(CancellationToken ct) => Forward(HttpMethod.Get, "organizations", ct);
    [HttpGet]
    public Task<IActionResult> List([FromQuery] string organizationTokenId, CancellationToken ct) =>
        Forward(HttpMethod.Get, "?organizationTokenId=" + Uri.EscapeDataString(organizationTokenId ?? ""), ct);
    [HttpPost]
    public Task<IActionResult> Create(CancellationToken ct) => Forward(HttpMethod.Post, "", ct);
    [HttpPut("{companyTokenId}")]
    public Task<IActionResult> Update(string companyTokenId, CancellationToken ct) =>
        Forward(HttpMethod.Put, Uri.EscapeDataString(companyTokenId), ct);

    private async Task<IActionResult> Forward(HttpMethod method, string suffix, CancellationToken ct)
    {
        if (!_current.IsAuthenticated || string.IsNullOrWhiteSpace(_current.OwnerUserTokenID)) return Unauthorized();
        if (method != HttpMethod.Get)
        {
            try { await _antiforgery.ValidateRequestAsync(HttpContext); }
            catch (AntiforgeryValidationException) { return BadRequest(new { success = false, message = "Invalid anti-forgery token." }); }
        }
        var baseUrl = _configuration["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var origin) || origin.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(origin.Query) || !string.IsNullOrEmpty(origin.Fragment) || origin.AbsolutePath != "/")
            return Problem(statusCode: 500, title: "Invalid AskPearlsApi:BaseUrl configuration.");
        string token;
        try { token = _jwt.CreateForCurrentUser(); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
        catch (InvalidOperationException) { return Problem(statusCode: 500, title: "Invalid API JWT configuration."); }
        using var request = new HttpRequestMessage(method, new Uri(origin, "api/account/companies" + (suffix.Length == 0 ? "" : suffix[0] == '?' ? suffix : "/" + suffix)));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (method == HttpMethod.Post || method == HttpMethod.Put)
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
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json; charset=utf-8",
                Content = await response.Content.ReadAsStringAsync(ct)
            };
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return Problem(statusCode: 504, title: "API timed out."); }
        catch (HttpRequestException) { return Problem(statusCode: 502, title: "Could not reach API."); }
    }
}
