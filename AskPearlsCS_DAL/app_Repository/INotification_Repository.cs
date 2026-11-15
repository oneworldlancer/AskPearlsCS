using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_DAL.app_Repository;

public interface INotification_Repository
{
    Task<NotificationPageDto> GetAsync(string owner, int skip, int take, CancellationToken ct);
    Task<int> UnreadCountAsync(string owner, CancellationToken ct);
    Task<bool> MarkReadAsync(string owner, long id, CancellationToken ct);
    Task<int> MarkAllReadAsync(string owner, CancellationToken ct);
    Task<NotificationDto> CreateAsync(NotificationCreateRequest request, CancellationToken ct);
    Task<NotificationPreferencesDto> GetPreferencesAsync(string owner, CancellationToken ct);
    Task SavePreferencesAsync(string owner, NotificationPreferencesDto preferences, CancellationToken ct);
}
