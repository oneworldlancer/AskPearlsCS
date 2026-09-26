using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPrivateshowTicketList
{
    public long PrivateShowTicketId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? PrivateShowId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ScreenId { get; set; }

    public string? PrivateShowSessionId { get; set; }

    public bool? IsPrivateShowMaxInviteCount { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
