using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserAccountUsagePeriodList
{
    public long UaccountUsagePeriodId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? UsagePeriodTokenId { get; set; }

    public DateTime? UsagePeriodStartUtc { get; set; }

    public DateTime? UsagePeriodEndUtc { get; set; }

    public string? MetricCode { get; set; }

    public string? MetricType { get; set; }

    public string? MetricText { get; set; }

    public long? UsedCount { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

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
