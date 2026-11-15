using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

//using AskPearlsCS_PORTAL.Services;

using Microsoft.AspNetCore.Mvc;

using AskPearlsCS_PORTAL.app_Manager;

namespace AskPearlsCS_PORTAL.Controllers;

public sealed class SupportContactRequest
{
    [Required, StringLength(120, MinimumLength = 2)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, StringLength(5000, MinimumLength = 10)] public string Message { get; set; } = "";
    [StringLength(150)] public string? Reference { get; set; }
    // Honeypot: bots may fill this invisible field.
    public string? Website { get; set; }
}

[Route("support/contact")]
public sealed class SupportContactController : Controller
{
    private static readonly ConcurrentDictionary<string, (DateTimeOffset Start, int Count)> Attempts = new();
    private readonly ISupportEmailService _sender;
    private readonly ILogger<SupportContactController> _logger;
    public SupportContactController(ISupportEmailService sender, ILogger<SupportContactController> logger)
    { _sender = sender; _logger = logger; }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromForm] SupportContactRequest request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Website)) return Ok(new { success = true });
        if (!ModelState.IsValid) return BadRequest(new { success = false, message = "Please enter a valid name, email and message (at least 10 characters)." });
        // Small in-memory guard for MVP. Use distributed ASP.NET Core rate limiting for multi-instance production deployments.
        var key = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTimeOffset.UtcNow;
        var entry = Attempts.AddOrUpdate(key, _ => (now, 1), (_, old) => now - old.Start > TimeSpan.FromMinutes(15) ? (now, 1) : (old.Start, old.Count + 1));
        if (entry.Count > 5) return StatusCode(429, new { success = false, message = "Too many requests. Please try again later." });
        try
        {
            await _sender.SendAsync(request.Name.Trim(), request.Email.Trim(), request.Message.Trim(), request.Reference?.Trim(), ct);
            return Ok(new { success = true, message = "Your support request has been sent successfully." });
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Support email delivery failed");
            return StatusCode(503, new { success = false, message = "Unable to send your request right now. Please try again later." });
        }
    }
}
