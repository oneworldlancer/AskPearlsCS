using System.Globalization;
using System.Text.Json;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class AccountSettings_Repository : IAccountSettings_Repository
{
    private readonly DBContextModel _db;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public AccountSettings_Repository(DBContextModel db) => _db = db;

    public async Task<AccountSettingsResponse?> GetAsync(long userId, CancellationToken ct = default)
    {
        var owner = userId.ToString(CultureInfo.InvariantCulture);
        // Google registration uses the numeric primary key as the authenticated user ID.
        // Do not require OwnerUserTokenId on the user row; older Google registrations may leave it null.
        var user = await _db.AskpearlsPortalUserLists.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive != false && x.IsVisible != false, ct);
        if (user is null) return null;

        var profile = await _db.AskpearlsPortalUserProfileLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false)
            .OrderByDescending(x => x.UprofileId).FirstOrDefaultAsync(ct);
        var organization = await _db.AskpearlsPortalUserOrganizationLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false)
            .OrderByDescending(x => x.UorganizationId).FirstOrDefaultAsync(ct);
        var subscription = await _db.AskpearlsPortalUserSubscriptionLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false)
            .OrderByDescending(x => x.UploadDate).FirstOrDefaultAsync(ct);
        var raw = await _db.AskpearlsPortalUserSettingLists.AsNoTracking()
            .Where(x => x.SettingCode == SettingsCode(owner) && x.IsActive == true)
            .OrderByDescending(x => x.SettingId).Select(x => x.SettingValue).FirstOrDefaultAsync(ct);
        var preferences = Deserialize(raw);
        var widgetCount = await _db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .CountAsync(x => x.OwnerUserTokenId == owner && x.IsActive == true && x.IsVisible == true, ct);

        var plan = string.IsNullOrWhiteSpace(subscription?.SubscriptionPlan) ? "Free" : subscription.SubscriptionPlan.Trim();
        var limit = WidgetLimit(plan);
        return new AccountSettingsResponse(true,
            Merge(preferences, user, profile, organization),
            new AccountSubscriptionDto(plan, subscription?.SubscriptionStatus ?? "preview", subscription is null,
                widgetCount, limit, limit.HasValue ? Math.Max(0, limit.Value - widgetCount) : null));
    }

    public async Task<AccountSettingsResponse?> SaveAsync(long userId, AccountSettingsUpdateRequest request, CancellationToken ct = default)
    {
        var owner = userId.ToString(CultureInfo.InvariantCulture);
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var user = await _db.AskpearlsPortalUserLists
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive != false && x.IsVisible != false, ct);
        if (user is null) return null;

        user.FirstName = request.FirstName;
        user.FirstNameLower = request.FirstName.ToLowerInvariant();
        user.LastName = request.LastName;
        user.LastNameLower = request.LastName.ToLowerInvariant();
        // IMPORTANT: never modify Email, EmailAddress or email verification flags.

        var profile = await _db.AskpearlsPortalUserProfileLists
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false)
            .OrderByDescending(x => x.UprofileId).FirstOrDefaultAsync(ct);
        if (profile is null)
        {
            profile = new AskpearlsPortalUserProfileList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                OwnerUserTokenId = owner,
                IsActive = true,
                IsDisplayToken = true,
                UploadDate = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserProfileLists.Add(profile);
        }
        profile.Occupation = request.JobTitle;
        profile.AboutMe = request.Bio;

        var organization = await _db.AskpearlsPortalUserOrganizationLists
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false)
            .OrderByDescending(x => x.UorganizationId).FirstOrDefaultAsync(ct);
        if (organization is null)
        {
            organization = new AskpearlsPortalUserOrganizationList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                OwnerUserTokenId = owner,
                AccountType = "portal",
                OrganizationTokenId = Guid.NewGuid().ToString("N"),
                ActiveStatus = "active",
                IsActive = true,
                IsVisible = true,
                UploadDate = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserOrganizationLists.Add(organization);
        }
        organization.OrganizationName = request.CompanyName;
        organization.CountryCode = request.Country;
        organization.CountryName = CountryName(request.Country);

        var setting = await _db.AskpearlsPortalUserSettingLists
            .Where(x => x.SettingCode == SettingsCode(owner) && x.IsActive == true)
            .OrderByDescending(x => x.SettingId).FirstOrDefaultAsync(ct);
        if (setting is null)
        {
            setting = new AskpearlsPortalUserSettingList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                SettingCode = SettingsCode(owner),
                SettingText = "AskPearls account preferences",
                SettingType = "json",
                SettingCategory = "decisionos.account",
                IsActive = true
            };
            _db.AskpearlsPortalUserSettingLists.Add(setting);
        }
        setting.SettingValue = JsonSerializer.Serialize(request, JsonOptions);
        setting.UploadDate = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        // Read after commit so the subscription display is populated consistently.
        var refreshed = await GetAsync(userId, ct);
        return refreshed is null ? null : refreshed with { Message = "Account settings saved." };
    }

    private static AccountSettingsUpdateRequest Deserialize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new AccountSettingsUpdateRequest();
        try { return JsonSerializer.Deserialize<AccountSettingsUpdateRequest>(raw, JsonOptions) ?? new(); }
        catch (JsonException) { return new(); }
    }

    private static AccountSettingsDto Merge(AccountSettingsUpdateRequest extra,
        AskpearlsPortalUserList user, AskpearlsPortalUserProfileList? profile,
        AskpearlsPortalUserOrganizationList? organization) => new()
        {
            FirstName = user.FirstName ?? extra.FirstName,
            LastName = user.LastName ?? extra.LastName,
            Email = user.EmailAddress ?? user.Email ?? "",
            JobTitle = profile?.Occupation ?? extra.JobTitle,
            Bio = profile?.AboutMe ?? extra.Bio,
            CompanyName = organization?.OrganizationName ?? extra.CompanyName,
            Country = organization?.CountryCode ?? extra.Country,
            Website = extra.Website,
            Industry = extra.Industry,
            CompanySize = extra.CompanySize,
            NonEssentialNotifications = extra.NonEssentialNotifications,
            EmailNotifications = extra.EmailNotifications,
            ActivityReports = extra.ActivityReports,
            ProductUpdates = extra.ProductUpdates,
            SecurityAlerts = true,
            Language = "en",
            Timezone = extra.Timezone,
            DateFormat = extra.DateFormat,
            WeekStart = extra.WeekStart,
            CompactMode = extra.CompactMode
        };

    private static string SettingsCode(string owner) => "decisionos.account.settings." + owner;
    private static string CountryName(string? code) => (code ?? "").ToUpperInvariant() switch
    {
        "EG" => "Egypt",
        "US" => "United States",
        "GB" => "United Kingdom",
        "DE" => "Germany",
        "FR" => "France",
        "CA" => "Canada",
        _ => code ?? ""
    };
    // UI estimate only. Actual entitlements must be enforced in widget creation service/repository.
    private static int? WidgetLimit(string plan) => plan.ToLowerInvariant() switch
    {
        "free" => 5,
        "starter" => 10,
        "professional" => 25,
        "premium" or "enterprise" => null,
        _ => 5
    };
}
