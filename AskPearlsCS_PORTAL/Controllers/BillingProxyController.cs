using System.Net.Http.Headers;
using AskPearlsCS_DOMAIN.app_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_PORTAL.Controllers;
[Authorize, Route("Account/Api/billing")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class BillingProxyController(IHttpClientFactory clients, IConfiguration configuration, ICurrentUserContext_Service current, IApiJwtToken_Service jwt) : Controller
{
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken ct)
    {
        if (!current.IsAuthenticated || string.IsNullOrWhiteSpace(current.OwnerUserTokenID) || current.OwnerUserTokenID.Trim() == "0") return Unauthorized();
        if (!Uri.TryCreate(configuration["AskPearlsApi:BaseUrl"]?.TrimEnd('/'), UriKind.Absolute, out var origin) || origin.Scheme != "https" || origin.AbsolutePath != "/" || origin.Query.Length != 0 || origin.Fragment.Length != 0 || origin.UserInfo.Length != 0) return Problem(statusCode: 500, title: "Invalid API origin");
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(origin, "api/account/billing/overview"));
        try { request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.CreateForCurrentUser()); }
        catch (UnauthorizedAccessException) { return Unauthorized(); }
        catch (InvalidOperationException) { return Problem(statusCode: 500, title: "JWT configuration invalid"); }
        try {
            using var response = await clients.CreateClient().SendAsync(request, ct);
            return new ContentResult { StatusCode = (int)response.StatusCode, ContentType = "application/json", Content = await response.Content.ReadAsStringAsync(ct) };
        } catch (HttpRequestException) { return Problem(statusCode: 502, title: "API unavailable"); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return Problem(statusCode: 504, title: "API timed out"); }
    }
}
