using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public sealed class Analytics_Service(IAnalytics_Repository repository) : IAnalytics_Service
{
    public Task<AnalyticsDashboardDto> GetAsync(string owner, int days, string? publicKey, string? activityType, CancellationToken ct)
    {
        if (days is not (7 or 30 or 90)) throw new ArgumentOutOfRangeException(nameof(days));
        if (activityType is not null and not ("all" or "started" or "completed" or "recommended" or "published")) throw new ArgumentException("Invalid activity type.");
        return repository.GetAsync(owner, days, publicKey, activityType, ct);
    }
}
