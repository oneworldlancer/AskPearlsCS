using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_DAL.app_Repository;

public interface IAnalytics_Repository { Task<AnalyticsDashboardDto> GetAsync(string owner, int days, string? publicKey, string? activityType, CancellationToken ct); }
