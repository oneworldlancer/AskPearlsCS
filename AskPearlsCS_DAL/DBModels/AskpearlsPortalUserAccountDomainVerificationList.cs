using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserAccountDomainVerificationList
{
    public long UwebsiteWidgetDomainVerificationId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? VerificationTokenId { get; set; }

    public string? DomainTokenId { get; set; }

    public string? MethodCode { get; set; }

    public string? MethodType { get; set; }

    public string? MethodText { get; set; }

    public string? MethodUrl { get; set; }

    public string? VerificationCode { get; set; }

    public string? VerificationType { get; set; }

    public string? VerificationText { get; set; }

    public string? VerificationStatus { get; set; }

    public string? VerificationUrl { get; set; }

    public string? VerificationWebUrl { get; set; }

    public string? VerificationHashTokenId { get; set; }

    public DateTime? RequestedAtUtc { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public DateTime? LastCheckedAtUtc { get; set; }

    public DateTime? VerifiedAtUtc { get; set; }

    public int? AttemptCount { get; set; }

    public string? LastFailureCode { get; set; }

    public string? LastFailureType { get; set; }

    public string? LastFailureText { get; set; }

    public string? LastFailureMessage { get; set; }

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
