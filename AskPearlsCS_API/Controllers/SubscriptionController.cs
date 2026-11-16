using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/account/subscription")]
public sealed class SubscriptionController : ControllerBase
{
    private readonly ISubscription_Service _subscription;
    private readonly ICurrentUserContext_Service _current;
    public SubscriptionController(ISubscription_Service subscription, ICurrentUserContext_Service current)
    { _subscription = subscription; _current = current; }

    // Call after registration or during authenticated MVP backfill.
    // Never accepts a plan, payment status, or owner ID from the browser.
    [HttpPost("ensure-free")]
    public async Task<IActionResult> EnsureFree(CancellationToken ct)
    {
        var owner = _current.OwnerUserTokenID;
        if (string.IsNullOrWhiteSpace(owner)) return Unauthorized();
        var ok = await _subscription.EnsureFreeAsync(owner, ct);
        return ok ? Ok(new { success = true }) : NotFound(new { success = false, message = "User not found." });
    }
}
