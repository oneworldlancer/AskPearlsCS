using System.Text.Json;
using System.Text.Json.Serialization;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Route("api/account/settings")]
public sealed class AccountSettingsController : ControllerBase
{
    private const string DemoOwnerUserTokenId = "1101";
    private const string SettingsCode = "decisionos.account.settings.1101";
    private readonly DBContextModel _db;
    private readonly IWebHostEnvironment _environment;

    public AccountSettingsController(DBContextModel db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var user = await _db.AskpearlsPortalUserLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UserId)
            .FirstOrDefaultAsync();
        var profile = await _db.AskpearlsPortalUserProfileLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true)
            .OrderByDescending(x => x.UprofileId)
            .FirstOrDefaultAsync();
        var organization = await _db.AskpearlsPortalUserOrganizationLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UorganizationId)
            .FirstOrDefaultAsync();
        var subscription = await _db.AskpearlsPortalUserSubscriptionLists.AsNoTracking()
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UploadDate)
            .FirstOrDefaultAsync();
        var storedSettings = await ReadStoredSettings();
        var widgetCount = await _db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .CountAsync(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true);

        var settings = MergeSettings(storedSettings, user, profile, organization);
        var planName = string.IsNullOrWhiteSpace(subscription?.SubscriptionPlan) ? "Free" : subscription.SubscriptionPlan.Trim();
        var limit = GetWidgetLimit(planName);
        return Ok(new
        {
            success = true,
            settings,
            subscription = new
            {
                plan = planName,
                status = subscription?.SubscriptionStatus ?? "preview",
                isIllustrative = subscription is null,
                widgetsCreated = widgetCount,
                widgetLimit = limit,
                widgetsRemaining = limit is null ? (int?)null : Math.Max(0, limit.Value - widgetCount)
            }
        });
    }

    [HttpPut]
    public async Task<IActionResult> SaveSettings([FromBody] AccountSettingsRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        if (request is null || !IsValid(request))
            return BadRequest(new { success = false, message = "Check the required profile fields and try again." });

        var user = await _db.AskpearlsPortalUserLists
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UserId)
            .FirstOrDefaultAsync();
        if (user is null)
            return NotFound(new { success = false, message = "The signed-in portal account was not found." });

        var profile = await _db.AskpearlsPortalUserProfileLists
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true)
            .OrderByDescending(x => x.UprofileId)
            .FirstOrDefaultAsync();
        if (profile is null)
        {
            profile = new AskpearlsPortalUserProfileList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                OwnerUserTokenId = DemoOwnerUserTokenId,
                IsActive = true,
                IsDisplayToken = true,
                UploadDate = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserProfileLists.Add(profile);
        }

        user.FirstName = request.FirstName.Trim();
        user.FirstNameLower = request.FirstName.Trim().ToLowerInvariant();
        user.LastName = request.LastName.Trim();
        user.LastNameLower = request.LastName.Trim().ToLowerInvariant();
        var emailChanged = !string.Equals(user.EmailAddress ?? user.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase);
        user.Email = request.Email.Trim();
        user.EmailAddress = request.Email.Trim();
        if (emailChanged)
        {
            user.IsValidateEmailAddress = false;
            user.IsEmailVerify = false;
        }
        profile.Occupation = Clean(request.JobTitle, 100);
        profile.AboutMe = Clean(request.Bio, 500);

        var organization = await _db.AskpearlsPortalUserOrganizationLists
            .Where(x => x.OwnerUserTokenId == DemoOwnerUserTokenId && x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UorganizationId)
            .FirstOrDefaultAsync();
        if (organization is null)
        {
            organization = new AskpearlsPortalUserOrganizationList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                AccountType = "portal",
                OwnerUserTokenId = DemoOwnerUserTokenId,
                OrganizationTokenId = Guid.NewGuid().ToString("N"),
                ActiveStatus = "active",
                IsActive = true,
                IsVisible = true,
                UploadDate = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserOrganizationLists.Add(organization);
        }
        organization.OrganizationName = request.CompanyName.Trim();
        organization.CountryCode = Clean(request.Country, 10);
        organization.CountryName = CountryName(request.Country);

        var stored = await _db.AskpearlsPortalUserSettingLists
            .FirstOrDefaultAsync(x => x.SettingCode == SettingsCode && x.IsActive == true);
        if (stored is null)
        {
            stored = new AskpearlsPortalUserSettingList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                SettingCode = SettingsCode,
                SettingText = "DecisionOS account preferences",
                SettingType = "json",
                SettingCategory = "decisionos.account",
                IsActive = true,
                UploadDate = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserSettingLists.Add(stored);
        }
        stored.SettingValue = JsonSerializer.Serialize(request);
        stored.UploadDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        var result = MergeSettings(request, user, profile, organization);
        return Ok(new { success = true, settings = result, message = "Account settings saved." });
    }

    private async Task<AccountSettingsRequest> ReadStoredSettings()
    {
        var raw = await _db.AskpearlsPortalUserSettingLists.AsNoTracking()
            .Where(x => x.SettingCode == SettingsCode && x.IsActive == true)
            .Select(x => x.SettingValue)
            .FirstOrDefaultAsync();
        if (string.IsNullOrWhiteSpace(raw)) return new AccountSettingsRequest();
        try { return JsonSerializer.Deserialize<AccountSettingsRequest>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AccountSettingsRequest(); }
        catch (JsonException) { return new AccountSettingsRequest(); }
    }

    private static AccountSettingsRequest MergeSettings(AccountSettingsRequest extras, AskpearlsPortalUserList? user,
        AskpearlsPortalUserProfileList? profile, AskpearlsPortalUserOrganizationList? organization) => extras with
        {
            FirstName = user?.FirstName ?? extras.FirstName,
            LastName = user?.LastName ?? extras.LastName,
            Email = user?.EmailAddress ?? user?.Email ?? extras.Email,
            JobTitle = profile?.Occupation ?? extras.JobTitle,
            Bio = profile?.AboutMe ?? extras.Bio,
            CompanyName = organization?.OrganizationName ?? extras.CompanyName,
            Country = organization?.CountryCode ?? extras.Country
        };

    private bool TryGetDemoOwner(out IActionResult? error)
    {
        error = null;
        if (!_environment.IsDevelopment()) { error = NotFound(); return false; }
        if (Request.Headers["X-Demo-Owner-User-Token"].ToString() != DemoOwnerUserTokenId)
        {
            error = Unauthorized(new { success = false, message = "Demo owner login is required." });
            return false;
        }
        return true;
    }

    private static bool IsValid(AccountSettingsRequest? value) => value is not null &&
        !string.IsNullOrWhiteSpace(value.FirstName) && value.FirstName.Trim().Length <= 60 &&
        !string.IsNullOrWhiteSpace(value.LastName) && value.LastName.Trim().Length <= 60 &&
        !string.IsNullOrWhiteSpace(value.Email) && value.Email.Trim().Length <= 150 &&
        value.Email.Contains('@') && !string.IsNullOrWhiteSpace(value.CompanyName) && value.CompanyName.Trim().Length <= 150 &&
        IsValidWebsite(value.Website) &&
        (value.Bio?.Length ?? 0) <= 500 && (value.JobTitle?.Length ?? 0) <= 100;

    private static bool IsValidWebsite(string? website)
    {
        if (string.IsNullOrWhiteSpace(website)) return true;
        return website.Length <= 200 && Uri.TryCreate(website, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static string? Clean(string? value, int max) => string.IsNullOrWhiteSpace(value) ? null : value.Trim()[..Math.Min(value.Trim().Length, max)];
    private static string CountryName(string? code) => (code ?? "").ToUpperInvariant() switch { "EG" => "Egypt", "US" => "United States", "GB" => "United Kingdom", "DE" => "Germany", "FR" => "France", "CA" => "Canada", _ => code ?? "" };
    private static int? GetWidgetLimit(string plan) => plan.ToLowerInvariant() switch { "free" => 5, "starter" => 10, "professional" => 25, "premium" or "enterprise" => null, _ => 5 };
}

public sealed record AccountSettingsRequest
{
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Email { get; init; } = "";
    public string JobTitle { get; init; } = "";
    public string Bio { get; init; } = "";
    public string CompanyName { get; init; } = "";
    public string Website { get; init; } = "";
    public string Industry { get; init; } = "technology";
    public string CompanySize { get; init; } = "2-10";
    public string Country { get; init; } = "EG";
    [JsonIgnore]
    public bool TwoFactor { get; init; }
    public bool NonEssentialNotifications { get; init; } = true;
    public bool EmailNotifications { get; init; } = true;
    public bool ActivityReports { get; init; } = true;
    public bool ProductUpdates { get; init; }
    public bool SecurityAlerts { get; init; } = true;
    public string Language { get; init; } = "en";
    public string Timezone { get; init; } = "Africa/Cairo";
    public string DateFormat { get; init; } = "dd/mm/yyyy";
    public string WeekStart { get; init; } = "monday";
    public bool CompactMode { get; init; }
}
