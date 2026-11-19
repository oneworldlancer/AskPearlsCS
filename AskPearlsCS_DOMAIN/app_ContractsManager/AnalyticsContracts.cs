namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record AnalyticsWidgetDto(string PublicKey, string Name, string Status, long Views, long Interactions, long Started, long Completed, long Recommendations, double CompletionRate);
public sealed record AnalyticsEventDto(string Id, string PublicKey, string WidgetName, string Type, string Title, string Detail, DateTime OccurredAtUtc);
public sealed record AnalyticsDayDto(string Date, long Views, long Interactions);
public sealed record AnalyticsTotalsDto(long Views, long Interactions, long Started, long Completed, long Recommendations, double CompletionRate);
public sealed record AnalyticsDashboardDto(AnalyticsTotalsDto Totals, AnalyticsTotalsDto Previous, IReadOnlyList<AnalyticsWidgetDto> Widgets, IReadOnlyList<AnalyticsDayDto> Chart, IReadOnlyList<AnalyticsEventDto> Activities);
