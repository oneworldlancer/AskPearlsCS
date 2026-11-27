using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSubscriptionBillingCheckoutList
{
    public long UsubscriptionBillingCheckoutId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? BillingCheckoutTokenId { get; set; }

    public string? SubscriptionTokenId { get; set; }

    public string? SubscriptionPlanProviderTokenId { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? SubscriptionPlanPriceTokenId { get; set; }

    public string? SubscriptionPlanTransactionTokenId { get; set; }

    public string? ProviderTransactionTokenId { get; set; }

    public string? ProviderCustomerTokenId { get; set; }

    public string? ProviderSubscriptionTokenId { get; set; }

    public string? BillingCheckoutStatusCode { get; set; }

    public string? BillingCheckoutStatusText { get; set; }

    public long? ExpectedAmountMinor { get; set; }

    public string? SubscriptionPlanTransactionTokenId9 { get; set; }

    public string? SubscriptionPlanTransactionTokenId8 { get; set; }

    public string? SubscriptionPlanTransactionTokenId7 { get; set; }

    public string? SubscriptionPlanTransactionTokenId6 { get; set; }

    public string? SubscriptionPlanTransactionTokenId5 { get; set; }

    public string? SubscriptionPlanTransactionTokenId4 { get; set; }

    public string? SubscriptionPlanTransactionTokenId3 { get; set; }

    public string? SubscriptionPlanTransactionTokenId2 { get; set; }

    public string? SubscriptionPlanTransactionTokenId1 { get; set; }

    public DateTime? CreatedUtc { get; set; }

    public DateTime? CompletedUtc { get; set; }

    public DateTime? ExpiresUtc { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? PaymentProviderCode { get; set; }

    public string? PaymentEnvironment { get; set; }

    public string? ProviderProductId { get; set; }

    public string? ProviderPriceId { get; set; }

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
