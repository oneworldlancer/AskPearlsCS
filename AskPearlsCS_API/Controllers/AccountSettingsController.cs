using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/account/settings")]
public sealed class AccountSettingsController : ControllerBase
{
    private readonly IAccountSettings_Service _settings;
    private readonly ICurrentUserContext_Service _currentUser;

    public AccountSettingsController(IAccountSettings_Service settings, ICurrentUserContext_Service currentUser)
    {
        _settings = settings;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings(CancellationToken ct)
    {
        if (_currentUser.UserId is not long userId)
            return Unauthorized(new { success = false, message = "Invalid authenticated AskPearls user." });
        var result = await _settings.GetAsync(userId, ct);
        return result is null
            ? NotFound(new { success = false, message = "The signed-in account was not found." })
            : Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> SaveSettings([FromBody] AccountSettingsUpdateRequest? request, CancellationToken ct)
    {
        if (_currentUser.UserId is not long userId)
            return Unauthorized(new { success = false, message = "Invalid authenticated AskPearls user." });
        try
        {
            var result = await _settings.SaveAsync(userId, request, ct);
            return result is null
                ? NotFound(new { success = false, message = "The signed-in account was not found." })
                : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
