using System.Net.Http.Headers;
using System.Net.Http.Json;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AskPearlsCS_PORTAL.Controllers;
[Authorize,Route("Account/Api/paddle")]
public sealed class PaddleProxyController(IHttpClientFactory clients,IConfiguration config,ICurrentUserContext_Service current,IApiJwtToken_Service jwt):Controller
{
    [HttpPost("checkout"),ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([FromBody] PaddleCheckoutRequest body,CancellationToken ct)
    {
        if(!current.IsAuthenticated || string.IsNullOrWhiteSpace(current.OwnerUserTokenID)||current.OwnerUserTokenID.Trim()=="0")return Unauthorized();
        if(!Uri.TryCreate(config["AskPearlsApi:BaseUrl"]?.TrimEnd('/'),UriKind.Absolute,out var origin)||origin.Scheme!="https"||origin.AbsolutePath!="/"||origin.Query.Length!=0||origin.Fragment.Length!=0||origin.UserInfo.Length!=0)return Problem(statusCode:500,title:"Invalid API origin");
        using var request=new HttpRequestMessage(HttpMethod.Post,new Uri(origin,"api/account/paddle/checkout"));
        request.Headers.Authorization=new AuthenticationHeaderValue("Bearer",jwt.CreateForCurrentUser());request.Content=JsonContent.Create(body);
        try{using var response=await clients.CreateClient().SendAsync(request,ct);return new ContentResult{StatusCode=(int)response.StatusCode,ContentType="application/json",Content=await response.Content.ReadAsStringAsync(ct)};}catch(HttpRequestException){return Problem(statusCode:502,title:"API unavailable");}
    }
}
