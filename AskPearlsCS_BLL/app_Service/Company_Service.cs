using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public sealed class Company_Service : ICompany_Service
{
    private readonly ICompany_Repository _repository;
    public Company_Service(ICompany_Repository repository) => _repository = repository;
    public Task<IReadOnlyList<OrganizationSummaryDto>> GetOrganizationsAsync(string owner, CancellationToken ct) =>
        _repository.GetOrganizationsAsync(Owner(owner), ct);
    public Task<IReadOnlyList<CompanyDto>> GetCompaniesAsync(string owner, string token, CancellationToken ct) =>
        _repository.GetCompaniesAsync(Owner(owner), Token(token), ct);
    public Task<CompanyDto?> CreateAsync(string owner, CompanySaveRequest request, CancellationToken ct) =>
        _repository.CreateAsync(Owner(owner), Validate(request), ct);
    public Task<CompanyDto?> UpdateAsync(string owner, string token, CompanySaveRequest request, CancellationToken ct) =>
        _repository.UpdateAsync(Owner(owner), Token(token), Validate(request), ct);
    private static string Owner(string? value) => Token(value);
    private static string Token(string? value) => string.IsNullOrWhiteSpace(value) || value.Trim() == "0" || value.Length > 200
        ? throw new ArgumentException("A valid token is required.") : value.Trim();
    private static CompanySaveRequest Validate(CompanySaveRequest? r)
    {
        if (r is null) throw new ArgumentException("Company details are required.");
        var name = r.CompanyName?.Trim() ?? "";
        if (name.Length is < 1 or > 150) throw new ArgumentException("Company name must be 1–150 characters.");
        static void Limit(string? value, int max, string field)
        { if (value?.Length > max) throw new ArgumentException(field + " is too long."); }
        Limit(r.CompanyIndustry, 60, "Industry"); Limit(r.CompanySize, 20, "Company size");
        Limit(r.CompanyCountry, 100, "Country"); Limit(r.CompanyCity, 100, "City");
        Limit(r.EmailAddress, 150, "Email"); Limit(r.PhoneNumber, 50, "Phone");
        static void Url(string? value, int max, bool httpsOnly)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (value.Length > max || !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                (httpsOnly ? uri.Scheme != Uri.UriSchemeHttps :
                    uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) ||
                string.IsNullOrWhiteSpace(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo))
                throw new ArgumentException("Enter a valid website or HTTPS logo URL.");
        }
        Url(r.CompanyWebsiteUrl, 200, false); Url(r.CompanyLogoUrl, 500, true);
        return r with { OrganizationTokenId = Token(r.OrganizationTokenId), CompanyName = name };
    }
}
