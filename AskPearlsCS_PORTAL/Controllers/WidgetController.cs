using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Widget")]
public sealed class WidgetController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WidgetController> _logger;

    public WidgetController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<WidgetController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    // POST /Widget/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromBody] CreateWidgetRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // This claim is added to the Portal Identity cookie after Google sign-in.
        var userId =
            User.FindFirst("AskPearlsUserId")?.Value ??
            User.FindFirst("PortalUserId")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(new { message = "The logged-in Portal user ID is missing." });

        var apiBaseUrl = _configuration["AskPearlsApi:BaseUrl"]?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(apiBaseUrl) ||
            !Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri) ||
            apiUri.Scheme != Uri.UriSchemeHttps)
        {
            _logger.LogError("AskPearlsApi:BaseUrl must be configured as an HTTPS URL.");
            return Problem("The AskPearls API URL is not configured correctly.");
        }

        var jwt = CreateApiToken(userId);

        var client = _httpClientFactory.CreateClient("AskPearlsApi");
        using var apiRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{apiBaseUrl}/api/widgets");

        apiRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

        apiRequest.Content = JsonContent.Create(request);

        try
        {
            using var apiResponse = await client.SendAsync(
                apiRequest,
                cancellationToken);

            var responseBody = await apiResponse.Content.ReadAsStringAsync(
                cancellationToken);

            if (!apiResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Widget API returned {StatusCode}: {Response}",
                    (int)apiResponse.StatusCode,
                    responseBody);

                return StatusCode((int)apiResponse.StatusCode, new
                {
                    message = "The Widget API could not create the widget.",
                    details = responseBody
                });
            }

            return Content(
                string.IsNullOrWhiteSpace(responseBody)
                    ? """{"message":"Widget created."}"""
                    : responseBody,
                "application/json");
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Could not reach the AskPearls Widget API.");
            return Problem("Could not connect to the AskPearls API.");
        }
    }

    private string CreateApiToken(string userId)
    {
        var issuer = _configuration["WidgetApiJwt:Issuer"]
            ?? throw new InvalidOperationException("WidgetApiJwt:Issuer is missing.");

        var audience = _configuration["WidgetApiJwt:Audience"]
            ?? throw new InvalidOperationException("WidgetApiJwt:Audience is missing.");

        var encodedKey = _configuration["WidgetApiJwt:SigningKey"]
            ?? throw new InvalidOperationException("WidgetApiJwt:SigningKey is missing.");

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(encodedKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "WidgetApiJwt:SigningKey must be a Base64-encoded key.",
                exception);
        }

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "WidgetApiJwt:SigningKey must decode to at least 32 bytes.");
        }

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("AskPearlsUserId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed record CreateWidgetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("websiteTokenId")] string WebsiteTokenId,
    [property: JsonPropertyName("websiteName")] string WebsiteName,
    [property: JsonPropertyName("websiteAddress")] string WebsiteAddress,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description);