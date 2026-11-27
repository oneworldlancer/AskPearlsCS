namespace AskPearlsCS_DOMAIN.app_ContractsManager;
public sealed class PaddleOptions
{
    public string Environment { get; set; } = "Sandbox";
    public string ApiKey { get; set; } = "";
    public string ClientSideToken { get; set; } = "";
    public string WebhookSecret { get; set; } = "";
    public bool Enabled { get; set; }
}
public sealed record PaddleCheckoutRequest(string PlanCode, string BillingInterval);
public sealed record PaddleCheckoutResult(string TransactionId, string ClientSideToken, string Environment);
public sealed record PaddleEvent(string EventId,string EventType,DateTimeOffset OccurredUtc,string Payload);
