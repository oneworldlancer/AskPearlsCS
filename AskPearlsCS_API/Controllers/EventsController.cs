//using AskPearls.Infrastructure;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_API.Controllers;

public record EventRequest(string PublicKey, string SessionId, string EventType, object Payload);

[ApiController]
[Route("api/events")]
public class EventsController  : ControllerBase
//public class EventsController(AskPearlsDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(EventRequest request)
    {
        //////////var widget = await db.Widgets.AsNoTracking()
        //////////    .FirstOrDefaultAsync(x => x.PublicKey == request.PublicKey);

        //////////if (widget is null) return NotFound();

        //////////db.Events.Add(new AskPearls.Domain.Event
        //////////{
        //////////    OrganizationId = widget.OrganizationId,
        //////////    WidgetId = widget.Id,
        //////////    SessionId = request.SessionId,
        //////////    EventType = request.EventType,
        //////////    PayloadJson = System.Text.Json.JsonSerializer.Serialize(request.Payload)
        //////////});

        //////////await db.SaveChangesAsync();
        return Ok();
    }
}
