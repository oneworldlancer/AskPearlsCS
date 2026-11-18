using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserAccountDomainList
{
    public long UaccountDomainId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? DomainTokenId { get; set; }

    public string? DomainCode { get; set; }

    public string? DomainType { get; set; }

    public string? DomainText { get; set; }

    public string? DomainNormalized { get; set; }

    public string? VerificationCode { get; set; }

    public string? VerificationType { get; set; }

    public string? VerificationText { get; set; }

    public string? VerificationStatus { get; set; }

    public DateTime? VerifiedAtUtc { get; set; }

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
