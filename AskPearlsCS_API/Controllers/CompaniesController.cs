using AskPearlsCS_BLL.app_Service;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "ApiJwt")]
[Route("api/account/companies")]
public sealed class CompaniesController : ControllerBase
{
    private readonly ICompany_Service _companies;
    private readonly ICurrentUserContext_Service _current;
    public CompaniesController(ICompany_Service companies, ICurrentUserContext_Service current)
    { _companies = companies; _current = current; }
    private string? Owner => _current.OwnerUserTokenID;

    [HttpGet("organizations")]
    public async Task<IActionResult> Organizations(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Owner)) return Unauthorized();
        return Ok(await _companies.GetOrganizationsAsync(Owner!, ct));
    }
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string organizationTokenId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Owner)) return Unauthorized();
        try { return Ok(await _companies.GetCompaniesAsync(Owner!, organizationTokenId, ct)); }
        catch (ArgumentException e) { return BadRequest(new { success = false, message = e.Message }); }
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CompanySaveRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Owner)) return Unauthorized();
        try
        {
            var result = await _companies.CreateAsync(Owner!, request, ct);
            return result is null ? NotFound(new { success = false, message = "Organization not found." }) : StatusCode(201, result);
        }
        catch (ArgumentException e) { return BadRequest(new { success = false, message = e.Message }); }
    }
    [HttpPut("{companyTokenId}")]
    public async Task<IActionResult> Update(string companyTokenId, [FromBody] CompanySaveRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Owner)) return Unauthorized();
        try
        {
            var result = await _companies.UpdateAsync(Owner!, companyTokenId, request, ct);
            return result is null ? NotFound(new { success = false, message = "Company not found." }) : Ok(result);
        }
        catch (ArgumentException e) { return BadRequest(new { success = false, message = e.Message }); }
    }
}
