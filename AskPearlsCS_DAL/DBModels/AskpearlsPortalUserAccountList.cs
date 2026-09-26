using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserAccountList
{
    public long UaccountId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? AccountType { get; set; }

    public bool? AccountIsEnable { get; set; }

    public bool? AccountIsVisible { get; set; }

    public int? ActionDay { get; set; }

    public int? ActionMonth { get; set; }

    public int? ActionYear { get; set; }

    public DateTime? ActionDate { get; set; }

    public bool? IsPersonal { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsBusiness { get; set; }

    public int? JoinDay { get; set; }

    public int? JoinMonth { get; set; }

    public int? JoinYear { get; set; }

    public int? ExitDay { get; set; }

    public int? ExitMonth { get; set; }

    public int? ExitYear { get; set; }

    public int? DatingJoinDay { get; set; }

    public int? DatingJoinMonth { get; set; }

    public int? DatingJoinYear { get; set; }

    public DateTime? DatingJoinDate { get; set; }

    public int? BusinessJoinDay { get; set; }

    public int? BusinessJoinMonth { get; set; }

    public int? BusinessJoinYear { get; set; }

    public DateTime? BusinessJoinDate { get; set; }

    public string? UserRole { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleValue { get; set; }

    public string? RoleName { get; set; }

    public string? RoleTitle { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public string? VisibleOnlineDateTimeMilliSec { get; set; }

    public bool? IsSystemUser { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
