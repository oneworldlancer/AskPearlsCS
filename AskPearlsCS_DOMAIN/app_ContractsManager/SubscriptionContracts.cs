namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record SubscriptionPlanDto(
    string Code, string Name, decimal MonthlyPriceUsd, int? WidgetLimit,
    string Description, bool IsAvailableForCheckout);

public sealed record SubscriptionCurrentDto(
    string Plan, string Status, int WidgetsCreated, int? WidgetLimit,
    int? WidgetsRemaining, bool IsProvisioned, string? SubscriptionTokenId,
    DateTime? CurrentPeriodStartUtc, DateTime? CurrentPeriodEndUtc,
    bool CancelAtPeriodEnd, bool CanCheckout);

public sealed record SubscriptionHistoryDto(
    string TokenId, string Plan, string Status, DateTime? CreatedAtUtc,
    DateTime? PeriodStartUtc, DateTime? PeriodEndUtc);
