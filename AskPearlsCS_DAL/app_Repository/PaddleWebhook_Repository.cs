using System.Data;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.EntityFrameworkCore;
namespace AskPearlsCS_DAL.app_Repository;
public sealed class PaddleWebhook_Repository(DBContextModel db) : IPaddleWebhook_Repository
{
    private IQueryable<AskpearlsPortalUserSubscriptionBillingWebhookEventList> Events => db.Set<AskpearlsPortalUserSubscriptionBillingWebhookEventList>().Where(x => x.PaymentProviderCode == "paddle" && x.WebhookEventEnvironment == "Sandbox" && x.AccountType == "portal" && x.IsActive == true);
    public async Task<PaddleWebhookWork?> ClaimAsync(CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var now = DateTime.UtcNow;
        var row = await Events.Where(x => x.ProcessingStatusCode == "pending" || (x.ProcessingStatusCode == "retry" && x.NextRetryUtc <= now) || (x.ProcessingStatusCode == "processing" && x.ProcessingLeaseUntilUtc <= now)).OrderBy(x => x.UsubscriptionBillingWebhookEventId).FirstOrDefaultAsync(ct);
        if (row == null) return null;
        var attempt = (row.ProcessingAttempts ?? 0) + 1;
        row.ProcessingAttempts = attempt;
        row.ProcessingStatusCode = attempt > 5 ? "failed" : "processing";
        row.ProcessingStartedUtc = now; row.LastAttemptUtc = now; row.ProcessingLeaseUntilUtc = now.AddMinutes(2);
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return attempt > 5 ? null : new(row.UsubscriptionBillingWebhookEventId, attempt, row.WebhookEventPayload ?? "");
    }
    private Task<AskpearlsPortalUserSubscriptionBillingWebhookEventList?> Lease(PaddleWebhookWork work, CancellationToken ct) => Events.SingleOrDefaultAsync(x => x.UsubscriptionBillingWebhookEventId == work.Id && x.ProcessingAttempts == work.Attempt && x.ProcessingStatusCode == "processing" && x.ProcessingLeaseUntilUtc > DateTime.UtcNow, ct);
    public async Task CompleteAsync(PaddleWebhookWork work, PaddleSubscriptionSnapshot? s, CancellationToken ct)
    {
        db.ChangeTracker.Clear();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var e = await Lease(work, ct); if (e == null) return;
        if (s != null)
        {
            var providers = db.Set<AskpearlsPortalUserSubscriptionBillingProviderList>();
            var p = await providers.SingleOrDefaultAsync(x => x.PaymentProviderCode == "paddle" && x.PaymentEnvironment == "Sandbox" && x.ProviderSubscriptionTokenId == s.Id, ct);
            if (p == null || p.LastEventTokenId == null || p.LastEventTokenId == "0" || p.LastEventUtc == null || p.LastEventUtc <= s.Updated)
            {
                var checkouts = db.Set<AskpearlsPortalUserSubscriptionBillingCheckoutList>();
                var c = p == null ? await checkouts.SingleOrDefaultAsync(x => x.PaymentProviderCode == "paddle" && x.PaymentEnvironment == "Sandbox" && x.AccountType == "portal" && x.BillingCheckoutTokenId == s.Checkout && x.ProviderPriceId == s.Price && x.ProviderSubscriptionTokenId == s.Id && x.BillingCheckoutStatusCode == "completed", ct) : null;
                if (p == null && (c == null || string.IsNullOrWhiteSpace(c.OwnerUserTokenId) || c.OwnerUserTokenId == "0" || c.ProviderCustomerTokenId != s.Customer)) throw new InvalidOperationException("Verified checkout mapping required.");
                if (p != null && (p.ProviderCustomerTokenId != s.Customer || p.SubscriptionPlanBillingProviderPriceTokenId != s.Price)) throw new InvalidOperationException("Customer or price change requires reconciliation.");
                if (p == null)
                {
                    p = new() { RowViewTokenId = Guid.NewGuid(), AccountType = "portal", OwnerUserTokenId = c!.OwnerUserTokenId, SubscriptionTokenId = Guid.NewGuid().ToString("N"), SubscriptionPlanTokenId = c.SubscriptionPlanTokenId, SubscriptionPlanProviderTokenId = c.SubscriptionPlanProviderTokenId, PaymentProviderCode = "paddle", PaymentEnvironment = "Sandbox", ProviderSubscriptionTokenId = s.Id, ProviderCustomerTokenId = s.Customer, SubscriptionPlanBillingProviderPriceTokenId = s.Price, SubscriptionPlanBillingProviderProductTokenId = c.ProviderProductId, CurrencyCode = c.CurrencyCode, BillingPeriodCode = c.BillingPeriodCode, CreatedUtc = DateTime.UtcNow, IsActive = true, IsVisible = true };
                    providers.Add(p); c.SubscriptionTokenId = p.SubscriptionTokenId;
                }
                p.BillingProviderSubscriptionStatusCode = s.Status; p.CurrentPeriodStartUtc = s.Start; p.CurrentPeriodEndUtc = s.End; p.NextBilledUtc = s.Next; p.ScheduledChangeAction = s.ScheduledAction; p.ScheduledChangeUtc = s.ScheduledAt; p.CancelAtPeriodEnd = s.ScheduledAction == "cancel"; p.LastEventTokenId = e.ProviderEventTokenId; p.LastEventUtc = s.Updated; p.ProviderPayload = s.Payload; p.UpdatedUtc = DateTime.UtcNow;
                e.OwnerUserTokenId = p.OwnerUserTokenId; e.ProviderSubscriptionTokenId = s.Id; e.ProviderCustomerTokenId = s.Customer;
            }
        }
        e.ProcessingStatusCode = s == null ? "ignored" : "processed"; e.ProcessedUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }
    public async Task FailAsync(PaddleWebhookWork work, CancellationToken ct)
    {
        db.ChangeTracker.Clear(); await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var e = await Lease(work, ct); if (e == null) return;
        e.ProcessingStatusCode = work.Attempt >= 5 ? "failed" : "retry";
        e.NextRetryUtc = DateTime.UtcNow.AddSeconds(new[]{30,120,300,900,900}[Math.Clamp(work.Attempt-1,0,4)]);
        e.LastErrorCode = "processing_error"; e.LastErrorMessage = "See restricted application logs or reconcile provider mapping.";
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }
}
