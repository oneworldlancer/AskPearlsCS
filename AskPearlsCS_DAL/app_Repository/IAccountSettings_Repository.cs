using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_DAL.app_Repository;

public interface IAccountSettings_Repository
{
    Task<AccountSettingsResponse?> GetAsync(long userId, CancellationToken ct = default);
    Task<AccountSettingsResponse?> SaveAsync(long userId, AccountSettingsUpdateRequest request, CancellationToken ct = default);
}
