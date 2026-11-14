using AskPearlsCS_DOMAIN.app_ContractsManager;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface IAccountSettings_Service
    {
        Task<AccountSettingsResponse?> GetAsync(string userId, CancellationToken ct = default);
        Task<AccountSettingsResponse?> SaveAsync(string userId, AccountSettingsUpdateRequest? request, CancellationToken ct = default);
    }
}
