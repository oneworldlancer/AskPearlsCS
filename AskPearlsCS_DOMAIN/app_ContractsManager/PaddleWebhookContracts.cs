namespace AskPearlsCS_DOMAIN.app_ContractsManager;
public sealed record PaddleWebhookWork(long Id, int Attempt, string Payload);
public sealed record PaddleSubscriptionSnapshot(string Id, string Customer, string Status, string Price, string? Checkout, DateTimeOffset? Start, DateTimeOffset? End, DateTimeOffset? Next, string? ScheduledAction, DateTimeOffset? ScheduledAt, DateTimeOffset Updated, string Payload);
