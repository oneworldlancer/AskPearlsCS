using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotification_Service _notifications;
    private readonly ICurrentUserContext_Service _user;
    public NotificationsController(INotification_Service notifications, ICurrentUserContext_Service user)
    { _notifications = notifications; _user = user; }
    private string? Owner => _user.OwnerUserTokenID is { Length: > 0 } s && s != "0" ? s : null;
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 30, CancellationToken ct = default) =>
        Owner is { } owner ? Ok(await _notifications.GetAsync(owner, skip, take, ct)) : Unauthorized();
    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct) =>
        Owner is { } owner ? Ok(new { count = await _notifications.UnreadCountAsync(owner, ct) }) : Unauthorized();
    [HttpPut("{id:long}/read")]
    public async Task<IActionResult> Read(long id, CancellationToken ct) =>
        Owner is { } owner ? (await _notifications.MarkReadAsync(owner, id, ct) ? NoContent() : NotFound()) : Unauthorized();
    [HttpPut("read-all")]
    public async Task<IActionResult> ReadAll(CancellationToken ct) =>
        Owner is { } owner ? Ok(new { updated = await _notifications.MarkAllReadAsync(owner, ct) }) : Unauthorized();
    [HttpGet("preferences")]
    public async Task<IActionResult> Preferences(CancellationToken ct) =>
        Owner is { } owner ? Ok(await _notifications.GetPreferencesAsync(owner, ct)) : Unauthorized();
    [HttpPut("preferences")]
    public async Task<IActionResult> SavePreferences([FromBody] NotificationPreferencesDto request, CancellationToken ct) =>
        Owner is { } owner ? Ok(await _notifications.SavePreferencesAsync(owner, request, ct)) : Unauthorized();
}
