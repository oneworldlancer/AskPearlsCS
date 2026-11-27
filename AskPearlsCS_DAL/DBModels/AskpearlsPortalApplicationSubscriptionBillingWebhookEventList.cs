using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationSubscriptionBillingWebhookEventList
{
    public long UsubscriptionBillingWebhookEventId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? CustomerUserTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? WebhookEventTokenId { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? WebhookEventCode { get; set; }

    public string? WebhookEventType { get; set; }

    public string? WebhookEventName { get; set; }

    public string? WebhookEventTitle { get; set; }

    public string? WebhookEventText { get; set; }

    public string? WebhookEventDescriptionShort { get; set; }

    public string? WebhookEventDescriptionLong { get; set; }

    public string? WebhookEventEnvironment { get; set; }

    public string? WebhookEventPayload { get; set; }

    public string? PaymentProviderCode { get; set; }

    public string? PaymentProviderCodeTitle { get; set; }

    public string? PaymentProviderCodeText { get; set; }

    public string? ProviderEventTokenId { get; set; }

    public string? ProviderNotificationTokenId { get; set; }

    public string? ProviderCustomerTokenId { get; set; }

    public string? ProviderSubscriptionTokenId { get; set; }

    public string? ProviderTransactionTokenId { get; set; }

    public string? ProcessingStatusCode { get; set; }

    public string? ProcessingStatusText { get; set; }

    public int? ProcessingAttempts { get; set; }

    public DateTime? ProcessingStartedUtc { get; set; }

    public DateTime? ProcessingLeaseUntilUtc { get; set; }

    public DateTime? LastAttemptUtc { get; set; }

    public DateTime? NextRetryUtc { get; set; }

    public string? LastErrorCode { get; set; }

    public string? LastErrorMessage { get; set; }

    public DateTimeOffset? OccurredUtc { get; set; }

    public DateTime? ReceivedUtc { get; set; }

    public DateTime? ProcessedUtc { get; set; }

    public bool? IsCheckoutEnabled { get; set; }

    public int? ItemOrder { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
