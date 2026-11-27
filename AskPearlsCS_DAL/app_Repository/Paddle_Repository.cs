using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace AskPearlsCS_DAL.app_Repository;
public interface IPaddle_Repository
{
    Task<string?> PriceAsync(string environment,string plan,string interval,CancellationToken ct);
    Task ReserveAsync(Guid id,string environment,string owner,string price,CancellationToken ct);
    Task AttachAsync(Guid id,string transaction,CancellationToken ct);
    Task StoreAsync(string environment,PaddleEvent evt,CancellationToken ct);
}
public sealed class Paddle_Repository(DBContextModel db) : IPaddle_Repository
{
    private IQueryable<AskpearlsPortalApplicationSubscriptionPlanPriceList> Prices(string environment) =>
        from p in db.Set<AskpearlsPortalApplicationSubscriptionPlanPriceList>()
        join plan in db.Set<AskpearlsPortalApplicationSubscriptionPlanList>() on p.SubscriptionPlanTokenId equals plan.SubscriptionPlanTokenId
        join provider in db.Set<AskpearlsPortalApplicationSubscriptionPlanProviderList>() on p.SubscriptionPlanProviderTokenId equals provider.SubscriptionPlanProviderTokenId
        where p.AccountType == "portal" && plan.AccountType == "portal" && provider.AccountType == "portal"
          && p.IsActive == true && p.IsVisible == true && p.IsCheckoutEnabled == true
          && plan.IsActive == true && plan.IsVisible == true && plan.IsCheckoutEnabled == true
          && provider.IsActive == true && provider.IsVisible == true && provider.IsCheckoutEnabled == true
          && provider.SubscriptionPlanTokenId == plan.SubscriptionPlanTokenId
          && provider.PaymentProviderCode == "paddle" && provider.PaymentEnvironment == environment
          && p.SubscriptionPlanBillingPaymentCode == "paddle" && p.SubscriptionPlanBillingPaymentEnvironment == environment
          && p.SubscriptionPlanBillingProviderProductTokenId == provider.ProviderProductTokenId
          && p.SubscriptionPlanPriceAmountMinor > 0 && p.SubscriptionPlanBillingIntervalFrequency == 1
        select p;
    public async Task<string?> PriceAsync(string environment,string plan,string interval,CancellationToken ct)
    {
        var rows=await (from p in Prices(environment) join s in db.Set<AskpearlsPortalApplicationSubscriptionPlanList>() on p.SubscriptionPlanTokenId equals s.SubscriptionPlanTokenId
            where s.SubscriptionPlanCode == plan && p.SubscriptionPlanBillingIntervalCode == interval
            select p.SubscriptionPlanBillingProviderPriceTokenId).Take(2).ToListAsync(ct);
        return rows.Count == 1 ? rows[0] : null;
    }
    public async Task ReserveAsync(Guid id,string environment,string owner,string price,CancellationToken ct)
    {
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var p=await Prices(environment).SingleAsync(x=>x.SubscriptionPlanBillingProviderPriceTokenId==price,ct);
        var now=DateTime.UtcNow;
        var checkouts=db.Set<AskpearlsPortalUserSubscriptionBillingCheckoutList>();
        if(await checkouts.AnyAsync(x=>x.OwnerUserTokenId==owner && x.PaymentProviderCode=="paddle" && x.PaymentEnvironment==environment && (x.BillingCheckoutStatusCode=="created" || x.BillingCheckoutStatusCode=="pending"),ct)) throw new InvalidOperationException("An existing checkout requires reconciliation.");
        if(await db.Set<AskpearlsPortalUserSubscriptionBillingProviderList>().AnyAsync(x=>x.OwnerUserTokenId==owner && x.PaymentProviderCode=="paddle" && x.PaymentEnvironment==environment && x.BillingProviderSubscriptionStatusCode!="canceled",ct)) throw new InvalidOperationException("Manage the existing subscription instead.");
        checkouts.Add(new(){RowViewTokenId=Guid.NewGuid(),AccountType="portal",OwnerUserTokenId=owner,BillingCheckoutTokenId=id.ToString("D"),SubscriptionPlanTokenId=p.SubscriptionPlanTokenId,SubscriptionPlanProviderTokenId=p.SubscriptionPlanProviderTokenId,SubscriptionPlanPriceTokenId=p.SubscriptionPlanPriceTokenId,ProviderPriceId=price,ProviderProductId=p.SubscriptionPlanBillingProviderProductTokenId,PaymentProviderCode="paddle",PaymentEnvironment=environment,BillingPeriodCode=p.SubscriptionPlanBillingIntervalCode,CurrencyCode=p.CurrencyCode,ExpectedAmountMinor=p.SubscriptionPlanPriceAmountMinor,BillingCheckoutStatusCode="created",CreatedUtc=now,CreatedDate=now,UpdatedDate=now,ExpiresUtc=now.AddHours(1),IsActive=true,IsVisible=true});
        await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
    }
    public async Task AttachAsync(Guid id,string transaction,CancellationToken ct)
    {
        var key=id.ToString("D");
        var row=await db.Set<AskpearlsPortalUserSubscriptionBillingCheckoutList>().SingleAsync(x=>x.BillingCheckoutTokenId==key && x.PaymentProviderCode=="paddle",ct);
        row.ProviderTransactionTokenId=transaction;row.BillingCheckoutStatusCode="pending";row.UpdatedDate=DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
    public async Task StoreAsync(string environment,PaddleEvent evt,CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(evt.EventId)||!evt.EventId.StartsWith("evt_",StringComparison.Ordinal)||string.IsNullOrWhiteSpace(evt.EventType)) throw new System.Text.Json.JsonException("Invalid event.");
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var rows=db.Set<AskpearlsPortalUserSubscriptionBillingWebhookEventList>();
        if(!await rows.AnyAsync(x=>x.PaymentProviderCode=="paddle" && x.WebhookEventEnvironment==environment && x.ProviderEventTokenId==evt.EventId,ct)){
            rows.Add(new(){RowViewTokenId=Guid.NewGuid(),AccountType="portal",WebhookEventTokenId=Guid.NewGuid().ToString("D"),PaymentProviderCode="paddle",WebhookEventEnvironment=environment,ProviderEventTokenId=evt.EventId,WebhookEventType=evt.EventType,WebhookEventPayload=evt.Payload,OccurredUtc=evt.OccurredUtc,ReceivedUtc=DateTime.UtcNow,ProcessingStatusCode="pending",ProcessingAttempts=0,IsActive=true,IsVisible=true});
            await db.SaveChangesAsync(ct);
        }
        await tx.CommitAsync(ct);
    }
}
