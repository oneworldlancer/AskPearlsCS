using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Helpers;

namespace AskPearlsCS_BLL.app_Service;

public sealed class AccountSettings_Service : IAccountSettings_Service
{
    private readonly IAccountSettings_Repository _repository;

    public AccountSettings_Service(IAccountSettings_Repository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task<AccountSettingsResponse?> GetAsync(
        string userId,
        CancellationToken ct = default)
    {
        return _repository.GetAsync(ValidateOwnerToken(userId), ct);
    }

    public Task<AccountSettingsResponse?> SaveAsync(
        string userId,
        AccountSettingsUpdateRequest? request,
        CancellationToken ct = default)
    {
        var ownerToken = ValidateOwnerToken(userId);

        if (request is null)
            throw new ArgumentException("Account settings are required.", nameof(request));

        // Clean only text fields. Never turn legitimate numeric zero values into blanks.
        var firstName = AppInputValueHelper.Clean(request.FirstName);
        var lastName = AppInputValueHelper.Clean(request.LastName);
        var companyName = AppInputValueHelper.Clean(request.CompanyName);
        var jobTitle = AppInputValueHelper.Clean(request.JobTitle);
        var bio = AppInputValueHelper.Clean(request.Bio);
        var website = AppInputValueHelper.Clean(request.Website);
        var logoUrl = AppInputValueHelper.Clean(request.OrganizationLogoUrl);
        var industry = AppInputValueHelper.Clean(request.Industry);
        var companySize = AppInputValueHelper.Clean(request.CompanySize);
        var country = AppInputValueHelper.Clean(request.Country);

        // These settings remain in the request contract for compatibility,
        // although the Preferences panel is hidden in the first MVP.
        var timezone = AppInputValueHelper.Clean(request.Timezone);
        var dateFormat = AppInputValueHelper.Clean(request.DateFormat);
        var weekStart = AppInputValueHelper.Clean(request.WeekStart);

        if (firstName.Length is < 1 or > 60 ||
            lastName.Length is < 1 or > 60 ||
            companyName.Length is < 1 or > 150 ||
            jobTitle.Length > 100 ||
            bio.Length > 500 ||
            industry.Length > 60 ||
            companySize.Length > 20 ||
            country.Length > 10 ||
            timezone.Length > 80 ||
            dateFormat.Length > 30 ||
            weekStart.Length > 20 ||
            !IsValidWebsite(website) ||
            !IsValidLogoUrl(logoUrl))
        {
            throw new ArgumentException(
                "Check the required profile and organization fields.",
                nameof(request));
        }

        var normalized = request with
        {
            FirstName = firstName,
            LastName = lastName,
            CompanyName = companyName,
            JobTitle = jobTitle,
            Bio = bio,
            Website = website,
            OrganizationLogoUrl = logoUrl,
            Industry = industry,
            CompanySize = companySize,
            Country = country,
            Timezone = timezone,
            DateFormat = dateFormat,
            WeekStart = weekStart,
            EmailNotifications = request.NonEssentialNotifications && request.EmailNotifications,
            ActivityReports = request.NonEssentialNotifications && request.ActivityReports,
            ProductUpdates = request.NonEssentialNotifications && request.ProductUpdates
        };

        // The DAL is responsible for converting empty optional text values
        // to the database's "0" placeholder where required.
        return _repository.SaveAsync(ownerToken, normalized, ct);
    }

    private static string ValidateOwnerToken(string? userId)
    {
        var token = userId?.Trim();
        if (string.IsNullOrWhiteSpace(token) || token == "0")
            throw new ArgumentException("A valid owner token is required.", nameof(userId));

        return token;
    }

    private static bool IsValidWebsite(string value)
    {
        return value.Length == 0 ||
               (value.Length <= 200 &&
                Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) &&
                !string.IsNullOrWhiteSpace(uri.Host) &&
                string.IsNullOrEmpty(uri.UserInfo));
    }

    private static bool IsValidLogoUrl(string value)
    {
        return value.Length == 0 ||
               (value.Length <= 500 &&
                Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                uri.Scheme == Uri.UriSchemeHttps &&
                !string.IsNullOrWhiteSpace(uri.Host) &&
                string.IsNullOrEmpty(uri.UserInfo));
    }
}
