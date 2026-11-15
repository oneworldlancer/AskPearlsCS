using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public interface ICompany_Service
{
    Task<IReadOnlyList<OrganizationSummaryDto>> GetOrganizationsAsync(string owner, CancellationToken ct);
    Task<IReadOnlyList<CompanyDto>> GetCompaniesAsync(string owner, string organizationToken, CancellationToken ct);
    Task<CompanyDto?> CreateAsync(string owner, CompanySaveRequest request, CancellationToken ct);
    Task<CompanyDto?> UpdateAsync(string owner, string companyToken, CompanySaveRequest request, CancellationToken ct);
}
