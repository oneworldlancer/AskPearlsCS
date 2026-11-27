using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationSubscriptionPlanPriceList
{
    public long UsubscriptionPlanPriceId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? SubscriptionPlanPriceTokenId { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? SubscriptionPlanBillingIntervalCode { get; set; }

    public string? SubscriptionPlanBillingIntervalTitle { get; set; }

    public string? SubscriptionPlanBillingIntervalName { get; set; }

    public string? SubscriptionPlanBillingIntervalText { get; set; }

    public string? SubscriptionPlanBillingCode { get; set; }

    public string? SubscriptionPlanBillingType { get; set; }

    public string? SubscriptionPlanBillingName { get; set; }

    public string? SubscriptionPlanBillingTitle { get; set; }

    public string? SubscriptionPlanBillingText { get; set; }

    public string? SubscriptionPlanBillingDescriptionShort { get; set; }

    public string? SubscriptionPlanBillingDescriptionLong { get; set; }

    public string? SubscriptionPlanBillingPaymentCode { get; set; }

    public string? SubscriptionPlanBillingPaymentEnvironment { get; set; }

    public string? SubscriptionPlanBillingProviderProductTokenId { get; set; }

    public string? SubscriptionPlanBillingProviderPriceTokenId { get; set; }

    public string? SubscriptionPlanBillingBillingPeriodCode { get; set; }

    public string? SubscriptionPlanBillingBillingPeriodName { get; set; }

    public string? SubscriptionPlanBillingBillingPeriodTitle { get; set; }

    public string? SubscriptionPlanBillingBillingPeriodText { get; set; }

    public string? SubscriptionPlanProviderTokenId { get; set; }

    public long? SubscriptionPlanPriceAmountMinor { get; set; }

    public int? SubscriptionPlanBillingIntervalFrequency { get; set; }

    public string? SubscriptionPlanBillingBillingPeriodText1 { get; set; }

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
