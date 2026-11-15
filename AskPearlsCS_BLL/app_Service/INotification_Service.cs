using AskPearlsCS_DOMAIN.app_ContractsManager;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface INotification_Service
    {
        Task<NotificationPageDto> GetAsync(string owner, int skip = 0, int take = 30, CancellationToken ct = default);
        Task<int> UnreadCountAsync(string owner, CancellationToken ct = default);
        Task<bool> MarkReadAsync(string owner, long id, CancellationToken ct = default);
        Task<int> MarkAllReadAsync(string owner, CancellationToken ct = default);
        Task<NotificationDto?> CreateAsync(NotificationCreateRequest request, CancellationToken ct = default);
        Task<NotificationPreferencesDto> GetPreferencesAsync(string owner, CancellationToken ct = default);
        Task<NotificationPreferencesDto> SavePreferencesAsync(string owner, NotificationPreferencesDto preferences, CancellationToken ct = default);
    }
}