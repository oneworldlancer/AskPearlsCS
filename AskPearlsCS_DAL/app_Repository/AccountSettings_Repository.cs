using System.Data;
using System.Globalization;
using System.Text.Json;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Helpers;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class AccountSettings_Repository : IAccountSettings_Repository
{
    private readonly DBContextModel _db;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public AccountSettings_Repository(DBContextModel db) => _db = db;
    private static string Store(string? s) => AppInputValueHelper.ToDatabaseValue(s);
    private static string Clean(string? s) => AppInputValueHelper.Clean(s);
    private static string Code(string owner, string section) => $"decisionos.account.{section}.{owner}";
    private static string LegacyCode(string owner) => "decisionos.account.settings." + owner;

    public async Task<AccountSettingsResponse?> GetAsync(string userId, CancellationToken ct = default)
    {
        var owner = userId.Trim();
        var user = await FindUserAsync(owner, ct);
        if (user is null) return null;
        var profile = await _db.AskpearlsPortalUserProfileLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false)
            .OrderByDescending(x => x.UprofileId).FirstOrDefaultAsync(ct);
        var organization = await _db.AskpearlsPortalUserOrganizationLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false)
            .OrderByDescending(x => x.UorganizationId).FirstOrDefaultAsync(ct);
        var legacy = await ReadJsonAsync<AccountSettingsUpdateRequest>(owner, LegacyCode(owner), ct);
        var profileJson = await ReadJsonAsync<ProfileJson>(owner, Code(owner, "profile"), ct);
        var orgJson = await ReadJsonAsync<OrganizationJson>(owner, Code(owner, "organization"), ct);
        var subscription = await _db.AskpearlsPortalUserSubscriptionLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false)
            .OrderByDescending(x => x.UsubscribeId).FirstOrDefaultAsync(ct);
        var count = await _db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .CountAsync(x => x.OwnerUserTokenId == owner && x.IsActive == true && x.IsVisible == true, ct);
        var plan = Clean(subscription?.SubscriptionPlan);
        if (plan.Length == 0) plan = "Free";
        var status = Clean(subscription?.SubscriptionStatus);
        if (status.Length == 0) status = subscription is null ? "not-provisioned" : "unknown";
        // Only an actual active subscription can grant paid entitlements.
        var effectivePlan = subscription is not null && status.Equals("active", StringComparison.OrdinalIgnoreCase)
            ? plan : "Free";
        var limit = WidgetLimit(effectivePlan);
        var settings = new AccountSettingsDto
        {
            FirstName = Clean(user.FirstName),
            LastName = Clean(user.LastName),
            Email = user.EmailAddress ?? user.Email ?? "",
            JobTitle = profile is not null ? Clean(profile.Occupation) : Pick(profileJson?.JobTitle, legacy?.JobTitle),
            Bio = profile is not null ? Clean(profile.AboutMe) : Pick(profileJson?.Bio, legacy?.Bio),
            CompanyName = organization is not null ? Clean(organization.OrganizationName) : Pick(orgJson?.OrganizationName, legacy?.CompanyName),
            OrganizationLogoUrl = organization is not null ? Clean(organization.OrganizationLogoUrl) : Pick(orgJson?.OrganizationLogoUrl, legacy?.OrganizationLogoUrl)
        };
        return new AccountSettingsResponse(true, settings,
            new AccountSubscriptionDto(effectivePlan, status, subscription is null,
                count, limit, limit.HasValue ? Math.Max(0, limit.Value - count) : null));
    }

    public async Task<AccountSettingsResponse?> SaveAsync(string userId, AccountSettingsUpdateRequest request, CancellationToken ct = default)
    {
        var owner = userId.Trim();
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var user = await FindUserAsync(owner, ct);
        if (user is null) return null;
        var now = DateTime.UtcNow;
        var section = request.SaveSection;
        if (section == "profile")
        {
            user.FirstName = Store(request.FirstName);
            user.FirstNameLower = user.FirstName.ToLowerInvariant();
            user.LastName = Store(request.LastName);
            user.LastNameLower = user.LastName.ToLowerInvariant();
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
                    UploadDate = now
                };
                _db.AskpearlsPortalUserProfileLists.Add(profile);
            }
            profile.Occupation = Store(request.JobTitle);
            profile.AboutMe = Store(request.Bio);
            await WriteJsonAsync(owner, section, new ProfileJson(
                Store(request.FirstName), Store(request.LastName), Store(request.JobTitle), Store(request.Bio)), ct);
        }
        else if (section == "organization")
        {
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
                    OrganizationTokenId = Guid.NewGuid().ToString("N"),
                    AccountType = "portal",
                    ActiveStatus = "active",
                    IsActive = true,
                    IsVisible = true,
                    UploadDate = now
                };
                _db.AskpearlsPortalUserOrganizationLists.Add(organization);
            }
            organization.OrganizationName = Store(request.CompanyName);
            organization.OrganizationLogoUrl = Store(request.OrganizationLogoUrl);
            // Company industry, size, country and website are intentionally NOT written here.
            await WriteJsonAsync(owner, section, new OrganizationJson(
                Store(request.CompanyName), Store(request.OrganizationLogoUrl)), ct);
        }
        else throw new ArgumentException("Unsupported account settings section.");
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        var response = await GetAsync(owner, ct);
        return response is null ? null : response with { Message = section + " saved." };
    }

    private async Task WriteJsonAsync<T>(string owner, string section, T payload, CancellationToken ct)
    {
        var code = Code(owner, section);
        var row = await _db.AskpearlsPortalUserSettingLists
            .Where(x => x.OwnerUserTokenId == owner && x.SettingCode == code && x.IsActive != false)
            .OrderByDescending(x => x.SettingId).FirstOrDefaultAsync(ct);
        if (row is null)
        {
            row = new AskpearlsPortalUserSettingList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = "0",
                OwnerUserTokenId = owner,
                SettingCode = code,
                SettingText = "AskPearls " + section + " settings",
                SettingType = "json",
                SettingCategory = "decisionos.account." + section,
                IsActive = true
            };
            _db.AskpearlsPortalUserSettingLists.Add(row);
        }
        row.SettingValue = JsonSerializer.Serialize(payload, JsonOptions);
        row.UploadDate = DateTime.UtcNow;
    }

    private async Task<T?> ReadJsonAsync<T>(string owner, string code, CancellationToken ct) where T : class
    {
        var raw = await _db.AskpearlsPortalUserSettingLists.AsNoTracking()
            .Where(x => (x.OwnerUserTokenId == owner || x.OwnerUserTokenId == null || x.OwnerUserTokenId == "0") &&
                        x.SettingCode == code && x.IsActive != false)
            .OrderByDescending(x => x.SettingId).Select(x => x.SettingValue).FirstOrDefaultAsync(ct);
        if (string.IsNullOrWhiteSpace(raw)) return null;
        try { return JsonSerializer.Deserialize<T>(raw, JsonOptions); }
        catch (JsonException) { return null; }
    }
    private static string Pick(params string?[] values)
    {
        foreach (var value in values) if (!string.IsNullOrWhiteSpace(value) && value.Trim() != "0") return Clean(value);
        return "";
    }
    private sealed record ProfileJson(string FirstName, string LastName, string JobTitle, string Bio);
    private sealed record OrganizationJson(string OrganizationName, string OrganizationLogoUrl);
    private static int? WidgetLimit(string plan) => plan.ToLowerInvariant() switch
    {
        "free" => 5,
        "starter" => 10,
        "professional" => 25,
        "premium" or "enterprise" => null,
        _ => 5
    };
    private async Task<AskpearlsPortalUserList?> FindUserAsync(string token, CancellationToken ct)
    {
        var user = await _db.AskpearlsPortalUserLists.FirstOrDefaultAsync(
            x => x.OwnerUserTokenId == token && x.IsActive != false && x.IsVisible != false, ct);
        if (user is not null) return user;
        if (!long.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var id)) return null;
        return await _db.AskpearlsPortalUserLists.FirstOrDefaultAsync(
            x => x.UserId == id && x.IsActive != false && x.IsVisible != false &&
                (x.OwnerUserTokenId == null || x.OwnerUserTokenId == ""), ct);
    }
}
