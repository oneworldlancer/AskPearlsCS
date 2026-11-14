using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_DAL.app_Repository;

public interface IAccountSettings_Repository
{
    Task<AccountSettingsResponse?> GetAsync(string userId, CancellationToken ct = default);
    Task<AccountSettingsResponse?> SaveAsync(string userId, AccountSettingsUpdateRequest request, CancellationToken ct = default);
}
