using System.Data;
using System.Globalization;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class Subscription_Repository : ISubscription_Repository
{
    private readonly DBContextModel _db;
    public Subscription_Repository(DBContextModel db) => _db = db;

    private IQueryable<AskpearlsPortalUserSubscriptionList> Rows(string owner) =>
        _db.AskpearlsPortalUserSubscriptionLists.Where(x =>
            x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false);

    private Task<bool> UserExists(string owner, CancellationToken ct) =>
        _db.AskpearlsPortalUserLists.AsNoTracking().AnyAsync(x =>
            x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false, ct);

    private static string Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim() == "0" ? "" : value.Trim();

    private static DateTime? Date(string? value) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed)
            ? parsed : null;

    private static bool IsActivePlan(AskpearlsPortalUserSubscriptionList x, DateTime now) =>
        Clean(x.SubscriptionStatus).Equals("active", StringComparison.OrdinalIgnoreCase) &&
        (Date(x.SubscriptionCurrentPeriodEndUtc) is not { } end || end > now);

    private static int? Limit(string plan) => plan.ToLowerInvariant() switch
    {
        "free" => 5,
        "starter" => 10,
        "professional" => 25,
        "premium" or "enterprise" => null,
        _ => 5
    };

    public async Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        if (!await UserExists(owner, ct)) return false;
        // Preserve historical subscriptions. Provision only if none exists at all;
        // restoring an expired paid account requires an explicit billing policy.
        if (!await _db.AskpearlsPortalUserSubscriptionLists.AsNoTracking()
            .AnyAsync(x => x.OwnerUserTokenId == owner, ct))
        {
            var now = DateTime.UtcNow;
            var row = new AskpearlsPortalUserSubscriptionList
            {
                RowViewTokenId = Guid.NewGuid(),
                RowDataTokenId = Guid.NewGuid().ToString("N"),
                OwnerUserTokenId = owner,
                OwnerMobileNumberTokenId = "0",
                OrganizationTokenId = "0",
                CompanyTokenId = "0",
                AccountType = "portal",
                SubscriptionTokenId = Guid.NewGuid().ToString("N"),
                SubscriptionCode = "free",
                SubscriptionType = "account",
                SubscriptionText = "Free AskPearls account (no payment)",
                SubscriptionPlan = "Free",
                SubscriptionStatus = "active",
                SubscriptionBillingInterval = "0",
                SubscriptionCancelAtPeriodEnd = "0",
                SubscriptionCurrentPeriodStartUtc = now.ToString("O"),
                SubscriptionCurrentPeriodEndUtc = "0",
                SubscriptionCreatedAtUtc = now.ToString("O"),
                SubscriptionUpdatedAtUtc = now.ToString("O"),
                SubscriptionStartDateTime = now,
                SubscriptionStartDateTimeCalendarValue = now,
                IsActive = true,
                IsVisible = true,
                ActiveStatus = "active",
                UploadDate = now
            };
            foreach (var property in typeof(AskpearlsPortalUserSubscriptionList).GetProperties())
            {
                if (property.PropertyType != typeof(string) || !property.CanWrite) continue;
                if (string.IsNullOrWhiteSpace((string?)property.GetValue(row))) property.SetValue(row, "0");
            }
            _db.AskpearlsPortalUserSubscriptionLists.Add(row);
            await _db.SaveChangesAsync(ct);
        }
        await tx.CommitAsync(ct);
        return true;
    }

    public async Task<SubscriptionCurrentDto?> GetCurrentAsync(string owner, CancellationToken ct = default)
    {
        if (!await UserExists(owner, ct)) return null;
        var rows = await Rows(owner).AsNoTracking()
            .OrderByDescending(x => x.UsubscribeId).ToListAsync(ct);
        var now = DateTime.UtcNow;
        var effective = rows.FirstOrDefault(x => IsActivePlan(x, now));
        var latest = rows.FirstOrDefault();
        var plan = effective is null ? "Free" : Clean(effective.SubscriptionPlan);
        if (string.IsNullOrEmpty(plan)) plan = "Free";
        var limit = Limit(plan);
        var used = await _db.AskpearlsPortalUserWebsiteWidgetLists.AsNoTracking()
            .CountAsync(x => x.OwnerUserTokenId == owner && x.IsActive == true && x.IsVisible == true, ct);
        return new SubscriptionCurrentDto(
            plan,
            effective is not null ? "active" : latest is null ? "not-provisioned" : "inactive",
            used, limit, limit.HasValue ? Math.Max(0, limit.Value - used) : null,
            latest is not null, Clean(effective?.SubscriptionTokenId) is { Length: > 0 } token ? token : null,
            Date(effective?.SubscriptionCurrentPeriodStartUtc), Date(effective?.SubscriptionCurrentPeriodEndUtc),
            Clean(effective?.SubscriptionCancelAtPeriodEnd) is "1" or "true", false);
    }

    public async Task<IReadOnlyList<SubscriptionHistoryDto>> GetHistoryAsync(string owner, CancellationToken ct = default)
    {
        if (!await UserExists(owner, ct)) return Array.Empty<SubscriptionHistoryDto>();
        var rows = await Rows(owner).AsNoTracking()
            .OrderByDescending(x => x.UsubscribeId).Take(100).ToListAsync(ct);
        return rows.Select(x => new SubscriptionHistoryDto(
            Clean(x.SubscriptionTokenId), Clean(x.SubscriptionPlan), Clean(x.SubscriptionStatus),
            Date(x.SubscriptionCreatedAtUtc), Date(x.SubscriptionCurrentPeriodStartUtc),
            Date(x.SubscriptionCurrentPeriodEndUtc))).ToArray();
    }
}
