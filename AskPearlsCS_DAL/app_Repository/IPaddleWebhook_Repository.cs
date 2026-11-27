using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_DAL.app_Repository;
public interface IPaddleWebhook_Repository
{
    Task<PaddleWebhookWork?> ClaimAsync(CancellationToken ct);
    Task CompleteAsync(PaddleWebhookWork work, PaddleSubscriptionSnapshot? snapshot, CancellationToken ct);
    Task FailAsync(PaddleWebhookWork work, CancellationToken ct);
}
