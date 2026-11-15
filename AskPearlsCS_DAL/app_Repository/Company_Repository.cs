using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class Company_Repository : ICompany_Repository
{
    private readonly DBContextModel _db;
    public Company_Repository(DBContextModel db) => _db = db;

    public async Task<IReadOnlyList<OrganizationSummaryDto>> GetOrganizationsAsync(string owner, CancellationToken ct)
    {
        var rows = await _db.AskpearlsPortalUserOrganizationLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false
                && x.OrganizationTokenId != null && x.OrganizationTokenId != "0")
            .OrderBy(x => x.UorganizationId).ToListAsync(ct);
        return rows.Select(x => new OrganizationSummaryDto(x.OrganizationTokenId!, Clean(x.OrganizationName))).ToList();
    }

    public async Task<IReadOnlyList<CompanyDto>> GetCompaniesAsync(string owner, string organizationToken, CancellationToken ct)
    {
        if (!await OwnsOrganization(owner, organizationToken, ct)) return Array.Empty<CompanyDto>();
        var rows = await _db.AskpearlsPortalUserCompanyLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.OrganizationTokenId == organizationToken
                && x.IsActive != false && x.IsVisible != false)
            .OrderBy(x => x.UcompanyId).ToListAsync(ct);
        return rows.Select(Map).ToList();
    }

    public async Task<CompanyDto?> CreateAsync(string owner, CompanySaveRequest request, CancellationToken ct)
    {
        if (!await OwnsOrganization(owner, request.OrganizationTokenId, ct)) return null;
        var row = new AskpearlsPortalUserCompanyList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = Guid.NewGuid().ToString("N"),
            OwnerUserTokenId = owner,
            OrganizationTokenId = request.OrganizationTokenId,
            CompanyTokenId = Guid.NewGuid().ToString("N"),
            AccountType = "portal",
            IsActive = true,
            IsVisible = true,
            ActiveStatus = "active",
            UploadDate = DateTime.UtcNow
        };
        Assign(row, request);
        _db.AskpearlsPortalUserCompanyLists.Add(row);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CompanyDto?> UpdateAsync(string owner, string companyToken, CompanySaveRequest request, CancellationToken ct)
    {
        if (!await OwnsOrganization(owner, request.OrganizationTokenId, ct)) return null;
        var row = await _db.AskpearlsPortalUserCompanyLists.FirstOrDefaultAsync(x =>
            x.OwnerUserTokenId == owner && x.CompanyTokenId == companyToken &&
            x.OrganizationTokenId == request.OrganizationTokenId && x.IsActive != false && x.IsVisible != false, ct);
        if (row is null) return null;
        Assign(row, request);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    private Task<bool> OwnsOrganization(string owner, string token, CancellationToken ct) =>
        _db.AskpearlsPortalUserOrganizationLists.AsNoTracking().AnyAsync(x =>
            x.OwnerUserTokenId == owner && x.OrganizationTokenId == token &&
            x.IsActive != false && x.IsVisible != false, ct);

    private static string Clean(string? value) => string.IsNullOrWhiteSpace(value) || value == "0" ? "" : value.Trim();
    private static string Store(string? value) => string.IsNullOrWhiteSpace(value) ? "0" : value.Trim();
    private static void Assign(AskpearlsPortalUserCompanyList row, CompanySaveRequest request)
    {
        row.CompanyName = Store(request.CompanyName);
        row.CompanyWebsiteUrl = Store(request.CompanyWebsiteUrl);
        row.CompanyLogoUrl = Store(request.CompanyLogoUrl);
        row.CompanyIndustry = Store(request.CompanyIndustry);
        row.CompanySize = Store(request.CompanySize);
        row.CompanyCountry = Store(request.CompanyCountry);
        row.CompanyCity = Store(request.CompanyCity);
        row.EmailAddress = Store(request.EmailAddress);
        row.PhoneNumber = Store(request.PhoneNumber);
    }
    private static CompanyDto Map(AskpearlsPortalUserCompanyList x) => new(
        Clean(x.CompanyTokenId), Clean(x.OrganizationTokenId), Clean(x.CompanyName),
        Clean(x.CompanyWebsiteUrl), Clean(x.CompanyLogoUrl), Clean(x.CompanyIndustry),
        Clean(x.CompanySize), Clean(x.CompanyCountry), Clean(x.CompanyCity),
        Clean(x.EmailAddress), Clean(x.PhoneNumber));
}
