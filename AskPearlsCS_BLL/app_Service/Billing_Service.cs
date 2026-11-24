using AskPearlsCS_DAL.app_Repository;
using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;
public sealed class Billing_Service(IBilling_Repository repository, ISubscription_Service subscription) : IBilling_Service
{
    public async Task<BillingOverviewDto?> GetOverviewAsync(string owner, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(owner) || owner.Trim() == "0") throw new ArgumentException("Owner required.");
        var current = await subscription.GetCurrentAsync(owner, ct);
        if (current is null) return null;
        var plans = await repository.GetPlansAsync(ct);
        var history = await subscription.GetHistoryAsync(owner, ct);
        return new(current, plans, history, false, "Payment provider not connected");
    }
}
