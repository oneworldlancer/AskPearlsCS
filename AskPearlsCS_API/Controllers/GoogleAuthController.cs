using AskPearlsCS_BLL.app_Service;

using Google.Apis.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Route("api/auth/google")]
public sealed class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuth_Service _googleAuthService;
    private readonly IConfiguration _configuration;

    public GoogleAuthController(
        IGoogleAuth_Service googleAuthService,
        IConfiguration configuration)
    {
        _googleAuthService = googleAuthService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("validate")]
    public async Task<IActionResult> Validate(
        [FromBody] GoogleTokenRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest("Google ID token is required.");

        var clientId = _configuration["GoogleOAuth:ClientId"];

        if (string.IsNullOrWhiteSpace(clientId))
            return Problem("Google OAuth Client ID is not configured.");

        try
        {
            var result = await _googleAuthService.ValidateAndProvisionAsync(
                request.IdToken,
                clientId,
                cancellationToken);

            return result is null
                ? Unauthorized("Google account could not be validated.")
                : Ok(result);
        }
        catch (InvalidJwtException)
        {
            return Unauthorized("Google ID token is invalid or expired.");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Conflict(ex.Message);
        }
    }
}

public sealed class GoogleTokenRequest
{
    public string IdToken { get; set; } = string.Empty;
}