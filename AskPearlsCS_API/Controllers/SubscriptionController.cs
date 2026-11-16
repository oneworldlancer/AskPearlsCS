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

    private string? Owner => _current.OwnerUserTokenID is { } value &&
        !string.IsNullOrWhiteSpace(value) && value.Trim() != "0" ? value.Trim() : null;

    [HttpGet("plans")]
    public IActionResult Plans() => Owner is null ? Unauthorized() : Ok(_subscription.GetPlans());

    [HttpGet("current")]
    public async Task<IActionResult> Current(CancellationToken ct)
    {
        if (Owner is not { } owner) return Unauthorized();
        var result = await _subscription.GetCurrentAsync(owner, ct);
        return result is null ? NotFound(new { success = false, message = "Account not found." }) : Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History(CancellationToken ct) =>
        Owner is { } owner ? Ok(await _subscription.GetHistoryAsync(owner, ct)) : Unauthorized();

    [HttpPost("ensure-free")]
    public async Task<IActionResult> EnsureFree(CancellationToken ct)
    {
        if (Owner is not { } owner) return Unauthorized();
        return await _subscription.EnsureFreeAsync(owner, ct)
            ? Ok(new { success = true })
            : NotFound(new { success = false, message = "User not found." });
    }

    // No plan-changing endpoint until trusted checkout + webhook verification exists.
    [HttpPost("checkout")]
    public IActionResult Checkout() => Owner is null ? Unauthorized() : Conflict(new
    {
        success = false,
        message = "Paid checkout is not configured. No subscription was changed."
    });
}
