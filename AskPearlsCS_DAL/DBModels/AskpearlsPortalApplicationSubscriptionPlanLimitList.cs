using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationSubscriptionPlanLimitList
{
    public long UsubscriptionPlanLimitId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? SubscriptionPlanTokenId { get; set; }

    public string? SubscriptionPlanCode { get; set; }

    public string? SubscriptionPlanLimitCode { get; set; }

    public string? SubscriptionPlanLimitName { get; set; }

    public string? SubscriptionPlanLimitTitle { get; set; }

    public string? SubscriptionPlanLimitText { get; set; }

    public string? SubscriptionPlanLimitDescription { get; set; }

    public string? SubscriptionPlanLimitResetPeriodCode { get; set; }

    public long? SubscriptionPlanLimitValue { get; set; }

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
