namespace AskPearlsCS_DOMAIN.app_ContractsManager;

// The email is deliberately absent from the write contract: Google owns it.
public sealed record AccountSettingsUpdateRequest
{
    public string FirstName { get; init; } = "0";
    public string LastName { get; init; } = "0";
    public string JobTitle { get; init; } = "0";
    public string Bio { get; init; } = "0";
    public string CompanyName { get; init; } = "0";
    public string Website { get; init; } = "0";
    public string Industry { get; init; } = "technology";
    public string CompanySize { get; init; } = "2-10";
    public string Country { get; init; } = "EG";
    public bool NonEssentialNotifications { get; init; } = true;
    public bool EmailNotifications { get; init; } = true;
    public bool ActivityReports { get; init; } = true;
    public bool ProductUpdates { get; init; }
    public string Timezone { get; init; } = "Africa/Cairo";
    public string DateFormat { get; init; } = "dd/mm/yyyy";
    public string WeekStart { get; init; } = "monday";
    public bool CompactMode { get; init; }
}

public sealed record AccountSettingsDto
{
    public string FirstName { get; init; } = "0";
    public string LastName { get; init; } = "0";
    public string Email { get; init; } = "0";
    public string JobTitle { get; init; } = "0";
    public string Bio { get; init; } = "0";
    public string CompanyName { get; init; } = "0";
    public string Website { get; init; } = "0";
    public string Industry { get; init; } = "technology";
    public string CompanySize { get; init; } = "2-10";
    public string Country { get; init; } = "EG";
    public bool NonEssentialNotifications { get; init; } = true;
    public bool EmailNotifications { get; init; } = true;
    public bool ActivityReports { get; init; } = true;
    public bool ProductUpdates { get; init; }
    public bool SecurityAlerts { get; init; } = true;
    public string Language { get; init; } = "en";
    public string Timezone { get; init; } = "Africa/Cairo";
    public string DateFormat { get; init; } = "dd/mm/yyyy";
    public string WeekStart { get; init; } = "monday";
    public bool CompactMode { get; init; }
}

public sealed record AccountSubscriptionDto(
    string Plan, string Status, bool IsIllustrative,
    int WidgetsCreated, int? WidgetLimit, int? WidgetsRemaining);

public sealed record AccountSettingsResponse(
    bool Success, AccountSettingsDto Settings, AccountSubscriptionDto? Subscription = null,
    string? Message = null);
