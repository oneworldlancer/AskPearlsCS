using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;

public sealed class AccountSettings_Service : IAccountSettings_Service
{
    private readonly IAccountSettings_Repository _repository;
    public AccountSettings_Service(IAccountSettings_Repository repository) => _repository = repository;

    public Task<AccountSettingsResponse?> GetAsync(long userId, CancellationToken ct = default)
    {
        if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
        return _repository.GetAsync(userId, ct);
    }

    public Task<AccountSettingsResponse?> SaveAsync(long userId, AccountSettingsUpdateRequest? request, CancellationToken ct = default)
    {
        if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
        if (request is null) throw new ArgumentException("Account settings are required.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Trim().Length > 60 ||
            string.IsNullOrWhiteSpace(request.LastName) || request.LastName.Trim().Length > 60 ||
            string.IsNullOrWhiteSpace(request.CompanyName) || request.CompanyName.Trim().Length > 150 ||
            (request.JobTitle?.Length ?? 0) > 100 || (request.Bio?.Length ?? 0) > 500 ||
            (request.Industry?.Length ?? 0) > 60 || (request.CompanySize?.Length ?? 0) > 20 ||
            (request.Country?.Length ?? 0) > 10 || (request.Timezone?.Length ?? 0) > 80 ||
            (request.DateFormat?.Length ?? 0) > 30 || (request.WeekStart?.Length ?? 0) > 20 ||
            !IsValidWebsite(request.Website))
            throw new ArgumentException("Check the required profile and organization fields.", nameof(request));

        // Optional notifications cannot be enabled when the master switch is off.
        var normalized = request with
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            CompanyName = request.CompanyName.Trim(),
            JobTitle = request.JobTitle?.Trim() ?? "",
            Bio = request.Bio?.Trim() ?? "",
            Website = request.Website?.Trim() ?? "",
            EmailNotifications = request.NonEssentialNotifications && request.EmailNotifications,
            ActivityReports = request.NonEssentialNotifications && request.ActivityReports,
            ProductUpdates = request.NonEssentialNotifications && request.ProductUpdates
        };
        return _repository.SaveAsync(userId, normalized, ct);
    }

    private static bool IsValidWebsite(string? website) => string.IsNullOrWhiteSpace(website) ||
        (website.Length <= 200 && Uri.TryCreate(website, UriKind.Absolute, out var uri) &&
         (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp));
}
