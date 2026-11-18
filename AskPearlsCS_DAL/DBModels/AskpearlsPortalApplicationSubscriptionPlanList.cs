using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationSubscriptionPlanList
{
    public long UsubscriptionPlanId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? SubscriptionPlanCode { get; set; }

    public string? SubscriptionPlanType { get; set; }

    public string? SubscriptionPlanName { get; set; }

    public string? SubscriptionPlanTitle { get; set; }

    public string? SubscriptionPlanText { get; set; }

    public string? SubscriptionPlanDescriptionShort { get; set; }

    public string? SubscriptionPlanDescriptionLong { get; set; }

    public decimal? SubscriptionPlanMonthlyPriceUsd { get; set; }

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
