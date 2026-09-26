using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastGroupshowVideoTicketList
{
    public long UgroupShowVideoTicketId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? GroupShowId { get; set; }

    public string? GroupShowTicketId { get; set; }

    public string? GroupShowTokenId { get; set; }

    public string? GroupShowVideoId { get; set; }

    public string? GroupShowVideoTokenId { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
