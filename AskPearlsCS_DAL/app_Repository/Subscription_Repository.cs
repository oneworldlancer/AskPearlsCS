using System.Data;

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class Subscription_Repository : ISubscription_Repository
{
    private readonly DBContextModel _db;
    public Subscription_Repository(DBContextModel db) => _db = db;

    public async Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default)
    {
        // A serializable transaction protects first-time creation against concurrent requests.
        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var exists = await _db.AskpearlsPortalUserLists.AsNoTracking().AnyAsync(x =>
            x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false, ct);
        if (!exists) return false;
        var hasAny = await _db.AskpearlsPortalUserSubscriptionLists.AsNoTracking()
            .AnyAsync(x => x.OwnerUserTokenId == owner, ct);
        if (!hasAny)
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
                SubscriptionEndDateTime = null, // Free has no expiry.
                SubscriptionEndDateTimeCalendarValue = null,
                IsActive = true,
                IsVisible = true,
                ActiveStatus = "active",
                UploadDate = now
            };
            // Existing legacy table has many optional string columns. Persist "0" for all
            // unset text values without changing DateTime, bool, or numeric fields.
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
}
