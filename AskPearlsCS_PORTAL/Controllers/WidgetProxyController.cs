using System.Net.Http.Headers;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Widget")]
public sealed class WidgetProxyController : Controller
{
    private readonly IHttpClientFactory _clients;
    private readonly IConfiguration _config;
    private readonly IAntiforgery _antiforgery;
    private readonly ICurrentUserContext_Service _currentUser;
    private readonly IApiJwtToken_Service _jwtService;

    public WidgetProxyController(
        IHttpClientFactory clients,
        IConfiguration config,
        IAntiforgery antiforgery,
        ICurrentUserContext_Service currentUser,
        IApiJwtToken_Service jwtService)
    {
        _clients = clients;
        _config = config;
        _antiforgery = antiforgery;
        _currentUser = currentUser;
        _jwtService = jwtService;
    }

    // Authenticated portal endpoints, mapped one-to-one to the supplied API controller.
    [HttpGet("Api/widgets")]
    public Task<IActionResult> GetMyWidgets(CancellationToken ct) => Forward(HttpMethod.Get, "widgets", ct);

    [HttpGet("Api/widgets/websites")]
    public Task<IActionResult> GetMyWebsites(CancellationToken ct) => Forward(HttpMethod.Get, "widgets/websites", ct);

    [HttpPost("Api/widgets")]
    public Task<IActionResult> CreateWidget(CancellationToken ct) => Forward(HttpMethod.Post, "widgets", ct);

    [HttpGet("Api/widgets/{publicKey}")]
    public Task<IActionResult> GetPublished(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey), ct);

    [HttpGet("Api/widgets/{publicKey}/design")]
    public Task<IActionResult> GetPublishedVersion(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "design"), ct);

    [HttpGet("Api/widgets/{publicKey}/questions")]
    public Task<IActionResult> GetPublishedQuestions(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "questions"), ct);

    [HttpGet("Api/widgets/{publicKey}/draft/configuration")]
    public Task<IActionResult> GetDesignDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "draft/configuration"), ct);

    [HttpPut("Api/widgets/{publicKey}/draft/configuration")]
    public Task<IActionResult> SaveDesignDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Put, KeyPath(publicKey, "draft/configuration"), ct);

    [HttpPost("Api/widgets/{publicKey}/publish")]
    public Task<IActionResult> PublishDesignDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Post, KeyPath(publicKey, "publish"), ct);

    [HttpGet("Api/widgets/{publicKey}/draft/questions")]
    public Task<IActionResult> GetQuestionDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "draft/questions"), ct);

    [HttpPut("Api/widgets/{publicKey}/draft/questions")]
    public Task<IActionResult> SaveQuestionDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Put, KeyPath(publicKey, "draft/questions"), ct);

    [HttpGet("Api/widgets/{publicKey}/draft/recommendations")]
    public Task<IActionResult> GetRecommendationDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "draft/recommendations"), ct);

    [HttpPut("Api/widgets/{publicKey}/draft/recommendations")]
    public Task<IActionResult> SaveRecommendationDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Put, KeyPath(publicKey, "draft/recommendations"), ct);

    [HttpGet("Api/widgets/{publicKey}/draft/rules")]
    public Task<IActionResult> GetDecisionRuleDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Get, KeyPath(publicKey, "draft/rules"), ct);

    [HttpPut("Api/widgets/{publicKey}/draft/rules")]
    public Task<IActionResult> SaveDecisionRuleDraft(string publicKey, CancellationToken ct) => Forward(HttpMethod.Put, KeyPath(publicKey, "draft/rules"), ct);

    private static string KeyPath(string key, string suffix = "") =>
        "widgets/" + Uri.EscapeDataString(key) + (suffix.Length == 0 ? "" : "/" + suffix);

    // Compatibility route for other /api/* endpoints, if required. Restrict to widgets only.
    [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE")]
    [Route("Api/widgets/{**rest}", Order = 100)]
    public Task<IActionResult> WidgetFallback(string? rest, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rest) || rest.Split('/').Any(x => x is "." or ".."))
            return Task.FromResult<IActionResult>(BadRequest(new { message = "Invalid widget path." }));
        return Forward(new HttpMethod(Request.Method), "widgets/" + rest, ct);
    }

    private async Task<IActionResult> Forward(HttpMethod method, string relativePath, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Unauthorized(new { success = false, message = "Missing or invalid AskPearlsUserId claim." });

        if (method != HttpMethod.Get && method != HttpMethod.Head)
        {
            try { await _antiforgery.ValidateRequestAsync(HttpContext); }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new { success = false, message = "Missing or invalid anti-forgery token." });
            }
        }

        var baseUrl = _config["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) || baseUri.Scheme != Uri.UriSchemeHttps ||
            !string.IsNullOrEmpty(baseUri.Query) || !string.IsNullOrEmpty(baseUri.Fragment))
            return Problem(statusCode: 500, title: "AskPearlsApi:BaseUrl must be an HTTPS base URL.");

        string jwt;
        try { jwt = _jwtService.CreateForCurrentUser(); }
        catch (UnauthorizedAccessException) { return Unauthorized(new { success = false, message = "Invalid authenticated user." }); }
        catch (InvalidOperationException) { return Problem(statusCode: 500, title: "API JWT configuration is invalid."); }

        // BaseUrl should be the API origin, e.g. https://api.askpearls.com (no /api suffix).
        var target = new Uri(baseUrl + "/api/" + relativePath + Request.QueryString);
        if (target.Host != baseUri.Host || target.Port != baseUri.Port || target.Scheme != baseUri.Scheme)
            return BadRequest(new { message = "Invalid API destination." });

        using var outgoing = new HttpRequestMessage(method, target);
        outgoing.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        if (Request.Headers.TryGetValue("Accept", out var accepts))
            outgoing.Headers.TryAddWithoutValidation("Accept", accepts.ToArray());

        if (method != HttpMethod.Get && method != HttpMethod.Head &&
            (Request.ContentLength is > 0 || Request.Headers.ContainsKey("Transfer-Encoding")))
        {
            outgoing.Content = new StreamContent(Request.Body);
            if (!string.IsNullOrWhiteSpace(Request.ContentType))
                outgoing.Content.Headers.TryAddWithoutValidation("Content-Type", Request.ContentType);
        }

        try
        {
            var client = _clients.CreateClient("AskPearlsWidgetApi");
            using var incoming = await client.SendAsync(outgoing, HttpCompletionOption.ResponseHeadersRead, ct);
            Response.StatusCode = (int)incoming.StatusCode;

            var apiJson = await incoming.Content.ReadAsStringAsync(ct);

            return new ContentResult
            {
                StatusCode = (int)incoming.StatusCode,
                ContentType = incoming.Content.Headers.ContentType?.ToString()
                              ?? "application/json; charset=utf-8",
                Content = apiJson
            };

            //////////var apiJson = await incoming.Content.ReadAsStringAsync(ct);

            //////////if (incoming.Content.Headers.ContentType != null)
            //////////    Response.ContentType = incoming.Content.Headers.ContentType.ToString();
            //////////if (incoming.Content.Headers.ContentDisposition != null)
            //////////    Response.Headers.ContentDisposition = incoming.Content.Headers.ContentDisposition.ToString();
            //////////await incoming.Content.CopyToAsync(Response.Body, ct);
            //////////return new EmptyResult();
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Problem(statusCode: 504, title: "AskPearls API request timed out.");
        }
        catch (HttpRequestException)
        {
            return Problem(statusCode: 502, title: "Could not reach the AskPearls API.");
        }
    }
}
