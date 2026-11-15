using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ContractsManager;

using Microsoft.EntityFrameworkCore;

namespace AskPearlsCS_DAL.app_Repository;

public sealed class Notification_Repository : INotification_Repository
{
    private readonly DBContextModel _db;
    public Notification_Repository(DBContextModel db) => _db = db;
    private IQueryable<AskpearlsPortalUserNotificationList> ForOwner(string owner) =>
        _db.Set<AskpearlsPortalUserNotificationList>()
            .Where(x => x.OwnerUserTokenId == owner && x.IsActive != false && x.IsVisible != false);
    private static string Clean(string? s) => string.IsNullOrWhiteSpace(s) || s.Trim() == "0" ? "" : s.Trim();
    private static string DbText(string? s) => Clean(s) is { Length: > 0 } v ? v : "0";
    private static NotificationDto Map(AskpearlsPortalUserNotificationList x) => new(
        x.UNotificationId, Clean(x.NotificationTokenId), Clean(x.NotificationCode),
        Clean(x.NotificationType), Clean(x.NotificationTitle), Clean(x.NotificationMessage),
        Clean(x.ActionUrl), x.IsRead == true, x.CreatedAtUtc, x.ReadAtUtc);

    public async Task<NotificationPageDto> GetAsync(string owner, int skip, int take, CancellationToken ct)
    {
        var query = ForOwner(owner).AsNoTracking();
        var total = await query.CountAsync(ct);
        var unread = await query.CountAsync(x => x.IsRead != true, ct);
        var rows = await query.OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.UNotificationId).Skip(skip).Take(take).ToListAsync(ct);
        return new(rows.Select(Map).ToArray(), unread, total);
    }
    public Task<int> UnreadCountAsync(string owner, CancellationToken ct) =>
        ForOwner(owner).CountAsync(x => x.IsRead != true, ct);

    public async Task<bool> MarkReadAsync(string owner, long id, CancellationToken ct)
    {
        var row = await ForOwner(owner).FirstOrDefaultAsync(x => x.UNotificationId == id, ct);
        if (row is null) return false;
        if (row.IsRead != true) { row.IsRead = true; row.ReadAtUtc = DateTime.UtcNow; await _db.SaveChangesAsync(ct); }
        return true;
    }
    public async Task<int> MarkAllReadAsync(string owner, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return await ForOwner(owner).Where(x => x.IsRead != true)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true)
                                      .SetProperty(x => x.ReadAtUtc, now), ct);
    }
    public async Task<NotificationDto> CreateAsync(NotificationCreateRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var row = new AskpearlsPortalUserNotificationList
        {
            RowViewTokenId = Guid.NewGuid(),
            RowDataTokenId = "0",
            OwnerUserTokenId = request.RecipientOwnerUserTokenId,
            OrganizationTokenId = DbText(request.OrganizationTokenId),
            CompanyTokenId = DbText(request.CompanyTokenId),
            NotificationTokenId = Guid.NewGuid().ToString("N"),
            NotificationCode = DbText(request.Code),
            NotificationValue = DbText(request.Value),
            NotificationType = DbText(request.Type),
            NotificationText = DbText(request.Message),
            NotificationTitle = DbText(request.Title),
            NotificationMessage = DbText(request.Message),
            ActionUrl = DbText(request.ActionUrl),
            IsRead = false,
            ReadAtUtc = null,
            CreatedAtUtc = now,
            IsActive = true,
            IsVisible = true,
            UploadDate = now,
            UploadDay = now.Day,
            UploadMonth = now.Month,
            UploadYear = now.Year
        };
        _db.Set<AskpearlsPortalUserNotificationList>().Add(row);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    private static readonly string[] Codes = {
        "notifications.enabled", "notifications.email.enabled", "notifications.activity.enabled",
        "notifications.product.enabled", "notifications.security.enabled" };
    private IQueryable<AskpearlsPortalUserSettingList> Settings(string owner) =>
        _db.AskpearlsPortalUserSettingLists.Where(x => x.OwnerUserTokenId == owner &&
            x.SettingCategory == "notifications" && x.IsActive != false);
    public async Task<NotificationPreferencesDto> GetPreferencesAsync(string owner, CancellationToken ct)
    {
        var rows = await Settings(owner).AsNoTracking().ToListAsync(ct);
        bool Get(int i, bool fallback)
        {
            var value = rows.LastOrDefault(x => x.SettingCode == Codes[i])?.SettingValue;
            return value is null ? fallback : value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        return new(Get(0, true), Get(1, true), Get(2, true), Get(3, false), true);
    }
    public async Task SavePreferencesAsync(string owner, NotificationPreferencesDto p, CancellationToken ct)
    {
        var rows = await Settings(owner).ToListAsync(ct);
        bool[] values = { p.NonEssentialNotifications, p.EmailNotifications, p.ActivityReports, p.ProductUpdates, true };
        for (var i = 0; i < Codes.Length; i++)
        {
            var row = rows.FirstOrDefault(x => x.SettingCode == Codes[i]);
            if (row is null)
            {
                row = new AskpearlsPortalUserSettingList
                {
                    RowViewTokenId = Guid.NewGuid(),
                    RowDataTokenId = "0",
                    OwnerUserTokenId = owner,
                    SettingCode = Codes[i],
                    SettingCategory = "notifications",
                    SettingType = "boolean",
                    IsActive = true,
                    UploadDate = DateTime.UtcNow
                };
                _db.AskpearlsPortalUserSettingLists.Add(row);
            }
            row.SettingValue = values[i] ? "1" : "0";
        }
        await _db.SaveChangesAsync(ct);
    }
}
