using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Helpers;

namespace AskPearlsCS_BLL.app_Service;

public sealed class AccountSettings_Service : IAccountSettings_Service
{
    private readonly IAccountSettings_Repository _repository;
    public AccountSettings_Service(IAccountSettings_Repository repository) => _repository = repository;

    public Task<AccountSettingsResponse?> GetAsync(string userId, CancellationToken ct = default) =>
        _repository.GetAsync(Owner(userId), ct);

    public Task<AccountSettingsResponse?> SaveAsync(string userId, AccountSettingsUpdateRequest? request, CancellationToken ct = default)
    {
        var owner = Owner(userId);
        if (request is null) throw new ArgumentException("Account settings are required.");
        var section = request.SaveSection?.Trim().ToLowerInvariant();
        if (section is not ("profile" or "organization"))
            throw new ArgumentException("SaveSection must be profile or organization.");
        var normalized = request with
        {
            SaveSection = section,
            FirstName = AppInputValueHelper.Clean(request.FirstName),
            LastName = AppInputValueHelper.Clean(request.LastName),
            JobTitle = AppInputValueHelper.Clean(request.JobTitle),
            Bio = AppInputValueHelper.Clean(request.Bio),
            CompanyName = AppInputValueHelper.Clean(request.CompanyName),
            OrganizationLogoUrl = AppInputValueHelper.Clean(request.OrganizationLogoUrl)
        };
        if (section == "profile")
        {
            if (normalized.FirstName.Length is < 1 or > 60 || normalized.LastName.Length is < 1 or > 60)
                throw new ArgumentException("First and last name must each be 1–60 characters.");
            if (normalized.JobTitle.Length > 100 || normalized.Bio.Length > 500)
                throw new ArgumentException("Job title or biography is too long.");
        }
        else
        {
            if (normalized.CompanyName.Length > 150)
                throw new ArgumentException("Organization name is too long.");
            var logo = normalized.OrganizationLogoUrl;
            if (logo.Length > 0 && (logo.Length > 500 ||
                !Uri.TryCreate(logo, UriKind.Absolute, out var uri) ||
                uri.Scheme != Uri.UriSchemeHttps || string.IsNullOrWhiteSpace(uri.Host) ||
                !string.IsNullOrEmpty(uri.UserInfo)))
                throw new ArgumentException("Enter a valid public HTTPS organization logo URL.");
        }
        return _repository.SaveAsync(owner, normalized, ct);
    }

    private static string Owner(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim() == "0"
            ? throw new ArgumentException("A valid authenticated owner is required.") : value.Trim();
}
