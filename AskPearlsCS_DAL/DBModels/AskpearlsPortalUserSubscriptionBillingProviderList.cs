using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSubscriptionBillingProviderList
{
    public long UsubscriptionBillingProviderId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? CustomerUserTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? SubscriptionTokenId { get; set; }

    public string? SubscriptionPlanProviderTokenId { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? SubscriptionPlanProviderCode { get; set; }

    public string? SubscriptionPlanProviderType { get; set; }

    public string? SubscriptionPlanProviderName { get; set; }

    public string? SubscriptionPlanProviderTitle { get; set; }

    public string? SubscriptionPlanProviderText { get; set; }

    public string? SubscriptionPlanProviderDescriptionShort { get; set; }

    public string? SubscriptionPlanProviderDescriptionLong { get; set; }

    public string? PaymentProviderCode { get; set; }

    public string? PaymentEnvironment { get; set; }

    public string? SubscriptionPlanBillingProviderProductTokenId { get; set; }

    public string? SubscriptionPlanBillingProviderPriceTokenId { get; set; }

    public string? BillingProviderSubscriptionStatusCode { get; set; }

    public string? BillingProviderSubscriptionStatusText { get; set; }

    public string? ProviderCustomerTokenId { get; set; }

    public string? ProviderSubscriptionTokenId { get; set; }

    public DateTimeOffset? CurrentPeriodStartUtc { get; set; }

    public DateTimeOffset? CurrentPeriodEndUtc { get; set; }

    public DateTimeOffset? NextBilledUtc { get; set; }

    public bool? CancelAtPeriodEnd { get; set; }

    public string? ScheduledChangeAction { get; set; }

    public DateTimeOffset? ScheduledChangeUtc { get; set; }

    public DateTimeOffset? CanceledUtc { get; set; }

    public DateTimeOffset? PausedUtc { get; set; }

    public string? LastEventTokenId { get; set; }

    public DateTimeOffset? LastEventUtc { get; set; }

    public string? ProviderPayload { get; set; }

    public DateTime? CreatedUtc { get; set; }

    public DateTime? UpdatedUtc { get; set; }

    public string? BillingProviderSubscriptionStatusText11 { get; set; }

    public string? BillingProviderSubscriptionStatusText1 { get; set; }

    public string? BillingPeriodCode { get; set; }

    public string? BillingPeriodName { get; set; }

    public string? BillingPeriodTitle { get; set; }

    public string? BillingPeriodText { get; set; }

    public string? CurrencyCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

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
