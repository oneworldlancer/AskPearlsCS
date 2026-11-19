using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public interface IAnalytics_Service { Task<AnalyticsDashboardDto> GetAsync(string owner, int days, string? publicKey, string? activityType, CancellationToken ct); }
