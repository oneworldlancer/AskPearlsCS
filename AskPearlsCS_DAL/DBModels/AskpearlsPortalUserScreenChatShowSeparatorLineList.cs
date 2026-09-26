using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowSeparatorLineList
{
    public long ScreenChatShowLineId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string ScreenChatShowTicketId { get; set; } = null!;

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? TempGroupId { get; set; }

    public string? UserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? ScreenChatShowLineCode { get; set; }

    public string? ScreenChatShowLineType { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
