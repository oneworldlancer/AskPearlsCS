using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
using AskPearlsCS_DOMAIN.app_Service;

namespace AskPearlsCS_BLL.app_Service;

public sealed class Notification_Service : INotification_Service
{
    private readonly INotification_Repository _repository;
    public Notification_Service(INotification_Repository repository) => _repository = repository;
    private static string Owner(string? owner) =>
        !string.IsNullOrWhiteSpace(owner) && owner.Trim() != "0"
            ? owner.Trim() : throw new ArgumentException("Invalid owner token.");
    public Task<NotificationPageDto> GetAsync(string owner, int skip = 0, int take = 30, CancellationToken ct = default) =>
        _repository.GetAsync(Owner(owner), Math.Max(0, skip), Math.Clamp(take, 1, 100), ct);
    public Task<int> UnreadCountAsync(string owner, CancellationToken ct = default) => _repository.UnreadCountAsync(Owner(owner), ct);
    public Task<bool> MarkReadAsync(string owner, long id, CancellationToken ct = default) =>
        id <= 0 ? Task.FromResult(false) : _repository.MarkReadAsync(Owner(owner), id, ct);
    public Task<int> MarkAllReadAsync(string owner, CancellationToken ct = default) => _repository.MarkAllReadAsync(Owner(owner), ct);
    public Task<NotificationPreferencesDto> GetPreferencesAsync(string owner, CancellationToken ct = default) =>
        _repository.GetPreferencesAsync(Owner(owner), ct);
    public async Task<NotificationPreferencesDto> SavePreferencesAsync(string owner, NotificationPreferencesDto preferences, CancellationToken ct = default)
    {
        var normalized = preferences with { SecurityAlerts = true };
        await _repository.SavePreferencesAsync(Owner(owner), normalized, ct);
        return normalized;
    }
    public async Task<NotificationDto?> CreateAsync(NotificationCreateRequest request, CancellationToken ct = default)
    {
        var recipient = Owner(request.RecipientOwnerUserTokenId);
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim() == "0" || request.Title.Length > 200 ||
            string.IsNullOrWhiteSpace(request.Message) || request.Message.Trim() == "0" || request.Message.Length > 4000 ||
            string.IsNullOrWhiteSpace(request.Code) || request.Code.Length > 100 ||
            string.IsNullOrWhiteSpace(request.Type) || request.Type.Length > 100)
            throw new ArgumentException("Invalid notification content.");
        var url = request.ActionUrl?.Trim();
        if (!string.IsNullOrEmpty(url) && url != "0" &&
            (!url.StartsWith('/') || url.StartsWith("//") || url.Contains('\\') || url.Any(char.IsControl)))
            throw new ArgumentException("ActionUrl must be a relative internal Portal path.");
        var preferences = await _repository.GetPreferencesAsync(recipient, ct);
        var type = request.Type.Trim().ToLowerInvariant();
        if (type != "security" && (!preferences.NonEssentialNotifications ||
            (type == "activity" && !preferences.ActivityReports) ||
            (type == "product" && !preferences.ProductUpdates))) return null;
        return await _repository.CreateAsync(request with { RecipientOwnerUserTokenId = recipient }, ct);
    }
}
