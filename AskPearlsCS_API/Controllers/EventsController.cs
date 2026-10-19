using System.Text.Json;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_API.Controllers;

public record EventRequest(string PublicKey, string WidgetSessionTokenId, string EventType, object Payload);

[ApiController]
[Route("api/events")]
public class EventsController(DBContextModel db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EventRequest request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.PublicKey) ||
            string.IsNullOrWhiteSpace(request.WidgetSessionTokenId) ||
            string.IsNullOrWhiteSpace(request.EventType))
        {
            return BadRequest(new
            {
                success = false,
                message = "PublicKey, WidgetSessionTokenId, and EventType are required."
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

        var sessionExists = await db.AskpearlsPortalUserWebsiteWidgetSessionLists
            .AsNoTracking()
            .AnyAsync(x =>
                x.WidgetSessionTokenId == request.WidgetSessionTokenId &&
                x.WidgetTokenId == widget.WidgetTokenId &&
                x.IsActive == true);

        if (!sessionExists)
        {
            return NotFound(new
            {
                success = false,
                message = "Active widget session was not found for this widget. Start a session first."
            });
        }

        var payloadJson = JsonSerializer.Serialize(request.Payload);
        var eventRow = new AskpearlsPortalUserWebsiteWidgetEventList
        {
            OrganizationTokenId = widget.OrganizationTokenId,
            OwnerUserTokenId = widget.OwnerUserTokenId,
            OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
            WebsiteTokenId = widget.WebsiteTokenId,
            WidgetTokenId = widget.WidgetTokenId,
            WidgetSessionTokenId = request.WidgetSessionTokenId,
            WidgetEventTokenId = Guid.NewGuid().ToString("N"),
            WidgetEventCode = request.EventType,
            WidgetEventType = request.EventType,
            WidgetEventPayloadJson = payloadJson,
            UserAgent = Request.Headers["User-Agent"].ToString(),
            Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            ReferrerUrl = Request.Headers["Referer"].ToString(),
            WidgetStatus = widget.WidgetStatusCode ?? widget.WidgetStatus,
            WidgetPublishedVersion = widget.WidgetPublishedVersion,
            IsNew = true,
            IsActive = true,
            IsVisible = true
        };

        db.AskpearlsPortalUserWebsiteWidgetEventLists.Add(eventRow);
        await db.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            eventId = eventRow.UwebsiteWidgetEventId,
            eventTokenId = eventRow.WidgetEventTokenId
        });
    }
}




////////////using AskPearls.Infrastructure;
//////////using AskPearlsCS_DAL.DBContext;

//////////using Microsoft.AspNetCore.Mvc;
//////////using Microsoft.EntityFrameworkCore;
////////////using Microsoft.EntityFrameworkCore;

//////////namespace AskPearlsCS_API.Controllers;

//////////public record EventRequest(string PublicKey, string SessionId, string EventType, object Payload);

//////////[ApiController]
//////////[Route("api/events")]
//////////public class EventsController(DBContextModel db) : ControllerBase
//////////{
//////////    [HttpPost]
//////////    public async Task<IActionResult> Post(EventRequest request)
//////////    {
//////////        var widget = await db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
//////////            .FirstOrDefaultAsync(x => x.WidgetPublicKey == request.PublicKey);

//////////        if (widget is null) return NotFound();

//////////        //////////db.AskpearlsPortalUserWebsiteWidgetEventLists.Add(new AskPearls.Domain.WidgetEvent
//////////        //////////{
//////////        //////////    OrganizationTokenId = widget.OrganizationTokenId,
//////////        //////////    WidgetTokenId = widget.WidgetTokenId,
//////////        //////////    SessionId = request.SessionId,
//////////        //////////    EventType = request.EventType //,
//////////        //////////    //PayloadJson = System.Text.Json.JsonSerializer.Serialize(request.Payload)
//////////        //////////});

//////////        ////////////await db.SaveChangesAsync();
//////////        return Ok();
//////////    }
//////////}
