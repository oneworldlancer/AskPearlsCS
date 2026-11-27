using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationSubscriptionPlanProviderList
{
    public long UsubscriptionPlanProviderId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

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

    public string? ProviderProductTokenId { get; set; }

    public string? ProviderPriceTokenId { get; set; }

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
