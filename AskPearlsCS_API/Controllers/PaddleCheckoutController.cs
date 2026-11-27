using AskPearlsCS_BLL.app_Service;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_API.Controllers;
[ApiController,Authorize(AuthenticationSchemes="ApiJwt"),Route("api/account/paddle")]
public sealed class PaddleCheckoutController(Paddle_Service paddle,ICurrentUserContext_Service current):ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] PaddleCheckoutRequest request,CancellationToken ct)
    {
        var owner=current.OwnerUserTokenID?.Trim();if(string.IsNullOrWhiteSpace(owner)||owner=="0")return Unauthorized();
        try{return Ok(await paddle.CheckoutAsync(owner,request,ct));}
        catch(ArgumentException){return BadRequest(new{message="Invalid or unavailable plan."});}
        catch(InvalidOperationException){return Conflict(new{message="Checkout unavailable."});}
        catch(HttpRequestException){return Problem(statusCode:502,title:"Payment provider unavailable.");}
    }
}
