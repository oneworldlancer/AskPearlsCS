using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_DAL.app_Repository;
public interface IBilling_Repository { Task<IReadOnlyList<BillingPlanDto>> GetPlansAsync(CancellationToken ct); }
