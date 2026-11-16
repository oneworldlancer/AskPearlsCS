using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;

public interface ISubscription_Service
{
    Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default);
    Task<SubscriptionCurrentDto?> GetCurrentAsync(string owner, CancellationToken ct = default);
    Task<IReadOnlyList<SubscriptionHistoryDto>> GetHistoryAsync(string owner, CancellationToken ct = default);
    IReadOnlyList<SubscriptionPlanDto> GetPlans();
}
