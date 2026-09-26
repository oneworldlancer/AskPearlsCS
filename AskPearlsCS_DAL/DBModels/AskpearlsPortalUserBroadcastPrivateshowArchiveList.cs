using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastPrivateshowArchiveList
{
    public long UprivateShowArchiveId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? PrivateShowId { get; set; }

    public string? PrivateShowTicketId { get; set; }

    public string? PrivateShowTokenId { get; set; }

    public string? PrivateShowVideoId { get; set; }

    public string? PrivateShowVideoTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CamFmsurl { get; set; }

    public string? PrivateShowSessionId { get; set; }

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
