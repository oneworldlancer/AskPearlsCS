namespace AskPearlsCS_DOMAIN.app_ContractsManager;
public sealed record BillingLimitDto(string Code, long? Value);
public sealed record BillingPlanDto(string Code, string Name, decimal? MonthlyPriceUsd, string Description, IReadOnlyList<BillingLimitDto> Limits);
public sealed record BillingOverviewDto(SubscriptionCurrentDto Current, IReadOnlyList<BillingPlanDto> Plans, IReadOnlyList<SubscriptionHistoryDto> History, bool CheckoutEnabled, string PaymentStatus);
