namespace AskPearlsCS_DOMAIN.app_ContractsManager;

// Google owns the email; only the selected section is writable.
public sealed record AccountSettingsUpdateRequest
{
    public string SaveSection { get; init; } = "profile";
    public string FirstName { get; init; } = "0";
    public string LastName { get; init; } = "0";
    public string JobTitle { get; init; } = "0";
    public string Bio { get; init; } = "0";
    // Kept for existing frontend compatibility: CompanyName means workspace name here.
    public string CompanyName { get; init; } = "0";
    public string OrganizationLogoUrl { get; init; } = "0";
}

public sealed record AccountSettingsDto
{
    public string FirstName { get; init; } = "0";
    public string LastName { get; init; } = "0";
    public string Email { get; init; } = "0";
    public string JobTitle { get; init; } = "0";
    public string Bio { get; init; } = "0";
    public string CompanyName { get; init; } = "0";
    public string OrganizationLogoUrl { get; init; } = "0";
    // Compatibility fields for existing page; no longer persisted to Organization.
    public string Website { get; init; } = "0";
    public string Industry { get; init; } = "0";
    public string CompanySize { get; init; } = "0";
    public string Country { get; init; } = "0";
    public bool NonEssentialNotifications { get; init; } = true;
    public bool EmailNotifications { get; init; } = true;
    public bool ActivityReports { get; init; } = true;
    public bool ProductUpdates { get; init; }
    public bool SecurityAlerts { get; init; } = true;
}

public sealed record AccountSubscriptionDto(
    string Plan, string Status, bool IsIllustrative,
    int WidgetsCreated, int? WidgetLimit, int? WidgetsRemaining);

public sealed record AccountSettingsResponse(
    bool Success, AccountSettingsDto Settings, AccountSubscriptionDto? Subscription = null,
    string? Message = null);
