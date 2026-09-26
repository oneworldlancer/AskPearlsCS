using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastGroupshowArchiveList
{
    public long UgroupShowArchiveId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? GroupShowId { get; set; }

    public string? GroupShowTicketId { get; set; }

    public string? GroupShowTokenId { get; set; }

    public string? GroupShowVideoId { get; set; }

    public string? GroupShowVideoTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CamFmsurl { get; set; }

    public string? GroupShowSessionId { get; set; }

    public int? ModelId { get; set; }

    public int? ModelSessionId { get; set; }

    public string? ModelType { get; set; }

    public string? ModelRole { get; set; }

    public int? MemberId { get; set; }

    public int? MemberSessionId { get; set; }

    public string? MemberType { get; set; }

    public string? MemberRole { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
