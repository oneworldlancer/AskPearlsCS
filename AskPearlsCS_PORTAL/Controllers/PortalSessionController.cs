
using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers
{
    [Authorize]
    [Route("Portal")]
    public class PortalSessionController : Controller
    {
        private readonly ICurrentUserContext_Service _currentUser;

        public PortalSessionController(
            ICurrentUserContext_Service currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet("KeepAlive")]
        public IActionResult KeepAlive()
        {
            if (!_currentUser.IsAuthenticated)
            {
                return Unauthorized(new
                {
                    authenticated = false,
                    reason = "NotAuthenticated"
                });
            }

            if (string.IsNullOrWhiteSpace(
                _currentUser.OwnerUserTokenID))
            {
                return Unauthorized(new
                {
                    authenticated = false,
                    reason = "MissingRequiredUserClaim"
                });
            }

            return Ok(new
            {
                authenticated = true,
                userContextValid = true,
                serverTime = DateTimeOffset.UtcNow
            });
        }
    }
}
