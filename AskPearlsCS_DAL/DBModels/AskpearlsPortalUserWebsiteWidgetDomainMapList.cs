using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserWebsiteWidgetDomainMapList
{
    public long UwebsiteWidgetDomainMapId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? WidgetDomainMapTokenId { get; set; }

    public string? DomainTokenId { get; set; }

    public string? WidgetPublicKey { get; set; }

    public DateTime? CreatedAtUtc { get; set; }

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
