using AskPearlsCS_BLL.app_Service;
using AskPearlsCS_DOMAIN.app_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_API.Controllers;
[ApiController, Authorize(AuthenticationSchemes = "ApiJwt"), Route("api/account/billing")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class BillingController(IBilling_Service billing, ICurrentUserContext_Service current) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken ct)
    {
        var owner = current.OwnerUserTokenID?.Trim();
        if (string.IsNullOrWhiteSpace(owner) || owner == "0") return Unauthorized();
        var result = await billing.GetOverviewAsync(owner, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
