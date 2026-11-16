using System.Text.Json.Serialization;

namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record WidgetAllowedDomainRequest(
    [property: JsonPropertyName("domain")] string Domain);
