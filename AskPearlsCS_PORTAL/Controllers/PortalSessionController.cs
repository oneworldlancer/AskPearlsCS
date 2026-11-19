using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AskPearlsCS_PORTAL.Controllers
{

    [Authorize]
    [Route("Portal")]
    public class PortalSessionController : Controller
    {
        [HttpGet("KeepAlive")]
        public IActionResult KeepAlive()
        {
            return Ok(new
            {
                authenticated = true,
                serverTime = DateTimeOffset.UtcNow
            });
        }
    }
}
