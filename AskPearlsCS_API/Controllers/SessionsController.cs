using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_API.Controllers;

public record WidgetSessionStartRequest(string PublicKey);

[ApiController]
[Route("api/sessions")]
public class SessionsController(DBContextModel db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Start([FromBody] WidgetSessionStartRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.PublicKey))
        {
            return BadRequest(new
            {
                success = false,
                message = "PublicKey is required."
            });
        }

        var widget = await db.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.WidgetPublicKey == request.PublicKey &&
                x.WidgetStatusCode == "published" &&
                x.IsVisible == true);

        if (widget is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Widget was not found."
            });
        }

        var publishedVersion = await db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.WidgetTokenId == widget.WidgetTokenId &&
                x.WidgetVersionNumber == widget.WidgetPublishedVersion &&
                x.WidgetVersionTokenId != null);

        if (publishedVersion is null)
        {
            return Conflict(new
            {
                success = false,
                message = "The widget has no matching published version."
            });
        }

        var sessionToken = Guid.NewGuid().ToString("N");
        var session = new AskpearlsPortalUserWebsiteWidgetSessionList
        {
            AccountType = widget.AccountType,
            OrganizationTokenId = widget.OrganizationTokenId,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            WebsiteTokenId = widget.WebsiteTokenId,
            WidgetTokenId = widget.WidgetTokenId,
            WidgetVersionTokenId = publishedVersion.WidgetVersionTokenId,
            SessionTokenId = sessionToken,
            WidgetSessionTokenId = sessionToken,
            IsGuest = true,
            IsActive = true,
            IsVisible = true
        };

        db.AskpearlsPortalUserWebsiteWidgetSessionLists.Add(session);
        await db.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            widgetSessionTokenId = sessionToken
        });
    }
}
