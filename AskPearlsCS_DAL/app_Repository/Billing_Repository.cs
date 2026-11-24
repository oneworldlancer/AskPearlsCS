using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using AskPearlsCS_DOMAIN.app_ContractsManager;
using Microsoft.EntityFrameworkCore;
namespace AskPearlsCS_DAL.app_Repository;
public sealed class Billing_Repository(DBContextModel db) : IBilling_Repository
{
    public async Task<IReadOnlyList<BillingPlanDto>> GetPlansAsync(CancellationToken ct)
    {
        var plans = await db.Set<AskpearlsPortalApplicationSubscriptionPlanList>().AsNoTracking()
            .Where(x => x.IsActive == true && x.IsVisible == true && x.AccountType == "portal")
            .OrderBy(x => x.ItemOrder).ToListAsync(ct);
        var limits = await db.Set<AskpearlsPortalApplicationSubscriptionPlanLimitList>().AsNoTracking()
            .Where(x => x.IsActive == true && x.IsVisible == true && x.AccountType == "portal")
            .OrderBy(x => x.ItemOrder).ToListAsync(ct);
        return plans.Where(p => !string.IsNullOrWhiteSpace(p.SubscriptionPlanCode)).Select(p => new BillingPlanDto(
            p.SubscriptionPlanCode!, p.SubscriptionPlanName ?? p.SubscriptionPlanCode!, p.SubscriptionPlanMonthlyPriceUsd,
            p.SubscriptionPlanDescriptionShort ?? "", limits.Where(l => l.SubscriptionPlanCode == p.SubscriptionPlanCode && l.SubscriptionPlanTokenId == p.SubscriptionPlanTokenId)
            .Select(l => new BillingLimitDto(l.SubscriptionPlanLimitCode ?? "", l.SubscriptionPlanLimitValue)).ToArray())).ToArray();
    }
}
