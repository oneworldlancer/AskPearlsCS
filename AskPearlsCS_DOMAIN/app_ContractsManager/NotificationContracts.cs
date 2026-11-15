namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record NotificationDto(
    long Id, string TokenId, string Code, string Type,
    string Title, string Message, string ActionUrl,
    bool IsRead, DateTime? CreatedAtUtc, DateTime? ReadAtUtc);

public sealed record NotificationPageDto(
    IReadOnlyList<NotificationDto> Items, int UnreadCount, int TotalCount);

public sealed record NotificationCreateRequest(
    string RecipientOwnerUserTokenId, string Code, string Type,
    string Title, string Message, string? ActionUrl = null,
    string? OrganizationTokenId = null, string? CompanyTokenId = null,
    string? Value = null);

public sealed record NotificationPreferencesDto(
    bool NonEssentialNotifications = true,
    bool EmailNotifications = true,
    bool ActivityReports = true,
    bool ProductUpdates = false,
    bool SecurityAlerts = true);
