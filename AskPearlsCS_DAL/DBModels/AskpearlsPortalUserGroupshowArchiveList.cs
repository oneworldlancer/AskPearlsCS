using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGroupshowArchiveList
{
    public long ArchiveGroupShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? GroupShowId { get; set; }

    public string? GroupShowTicketId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CamFmsurl { get; set; }

    public string? GroupShowSessionId { get; set; }

    public string? ModelId { get; set; }

    public string? ModelSessionId { get; set; }

    public string? ModelType { get; set; }

    public string? ModelRole { get; set; }

    public string? MemberId { get; set; }

    public string? MemberSessionId { get; set; }

    public string? MemberType { get; set; }

    public string? MemberRole { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
