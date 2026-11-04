using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AskPearlsCS_PORTAL.Controllers;

[Authorize]
[Route("Widget")]
[AutoValidateAntiforgeryToken]
public sealed class WidgetController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WidgetController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE")]
    [Route("Api/{**apiPath}")]
    public async Task<IActionResult> Api(
        string? apiPath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(apiPath) ||
            !apiPath.StartsWith("widgets", StringComparison.OrdinalIgnoreCase) ||
            apiPath.Contains("..", StringComparison.Ordinal) ||
            apiPath.Contains('\\'))
        {
            return BadRequest(new { message = "Invalid Widget API route." });
        }

        // This claim is added by the Portal after Google sign-in is validated.
        // Do not use an ID from the browser request.
        var userId = User.FindFirst("AskPearlsUserId")?.Value;

        if (!long.TryParse(userId, out var parsedUserId) || parsedUserId <= 0)
        {
            return Unauthorized(new { message = "The authenticated Portal user ID is missing." });
        }

        var apiBaseUrl = _configuration["AskPearlsApi:BaseUrl"];
        if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiBaseUri) ||
            apiBaseUri.Scheme != Uri.UriSchemeHttps)
        {
            return Problem("AskPearlsApi:BaseUrl must be configured as an HTTPS URL.");
        }

        var issuer = _configuration["ApiJwt:Issuer"];
        var audience = _configuration["ApiJwt:Audience"];
        var SigningKey = _configuration["ApiJwt:SigningKey"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(SigningKey))
        {
            return Problem("The Portal-to-API JWT settings are not configured.");
        }

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(SigningKey);
        }
        catch (FormatException)
        {
            return Problem("ApiJwt:SigningKey is not valid Base64.");
        }

        if (keyBytes.Length < 32)
        {
            return Problem("The JWT signing key must contain at least 32 bytes.");
        }

        var now = DateTime.UtcNow;
        var userIdText = parsedUserId.ToString();

        var claims = new[]
        {
            new Claim("AskPearlsUserId", userIdText),
            new Claim(JwtRegisteredClaimNames.Sub, userIdText),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(2),
            signingCredentials: credentials);

        var bearerToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        var upstreamUrl =
            $"{apiBaseUri.ToString().TrimEnd('/')}/api/{apiPath}{Request.QueryString}";

        using var upstreamRequest = new HttpRequestMessage(
            new HttpMethod(Request.Method),
            upstreamUrl);

        upstreamRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                bearerToken);

        if (Request.ContentLength > 0 || Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            upstreamRequest.Content = new StreamContent(Request.Body);

            if (!string.IsNullOrWhiteSpace(Request.ContentType))
            {
                upstreamRequest.Content.Headers.TryAddWithoutValidation(
                    "Content-Type",
                    Request.ContentType);
            }
        }

        try
        {
            var client = _httpClientFactory.CreateClient();

            using var upstreamResponse = await client.SendAsync(
                upstreamRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            var responseBody = await upstreamResponse.Content.ReadAsStringAsync(
                cancellationToken);

            return new ContentResult
            {
                StatusCode = (int)upstreamResponse.StatusCode,
                ContentType = upstreamResponse.Content.Headers.ContentType?.ToString()
                              ?? "application/json",
                Content = responseBody
            };
        }
        catch (HttpRequestException)
        {
            return Problem("The AskPearls API could not be reached.");
        }
    }
}