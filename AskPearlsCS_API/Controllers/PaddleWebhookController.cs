using AskPearlsCS_BLL.app_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_API.Controllers;
[ApiController,AllowAnonymous,Route("api/webhooks/paddle")]
public sealed class PaddleWebhookController(Paddle_Service paddle):ControllerBase
{
    [HttpPost,RequestSizeLimit(1048576)]
    public async Task<IActionResult> Receive(CancellationToken ct)
    {
        using var stream=new MemoryStream();await Request.Body.CopyToAsync(stream,ct);var body=stream.ToArray();
        if(!paddle.Verify(body,Request.Headers["Paddle-Signature"].ToString()))return Unauthorized();
        try{await paddle.ReceiveAsync(body,ct);}catch(System.Text.Json.JsonException){return BadRequest();}
        return Ok();
    }
}
