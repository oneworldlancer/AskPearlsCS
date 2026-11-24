using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;
public interface IBilling_Service { Task<BillingOverviewDto?> GetOverviewAsync(string owner, CancellationToken ct); }
