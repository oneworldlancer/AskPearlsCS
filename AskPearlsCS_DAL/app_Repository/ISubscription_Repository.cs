using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_DAL.app_Repository;

public interface ISubscription_Repository
{
    Task<bool> EnsureFreeAsync(string owner, CancellationToken ct = default);
    Task<SubscriptionCurrentDto?> GetCurrentAsync(string owner, CancellationToken ct = default);
    Task<IReadOnlyList<SubscriptionHistoryDto>> GetHistoryAsync(string owner, CancellationToken ct = default);
}
