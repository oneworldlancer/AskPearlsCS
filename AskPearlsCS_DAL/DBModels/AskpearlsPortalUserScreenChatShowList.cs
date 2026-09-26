using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowList
{
    public long ScreenChatShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? TlknUserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? ScreenUserType { get; set; }

    public string? UserType { get; set; }

    public string? GroupUserList { get; set; }

    public string? ShowId { get; set; }

    public string? ShowSessionId { get; set; }

    public string? ShowTicketId { get; set; }

    public string? ShowCode { get; set; }

    public string? ShowType { get; set; }

    public string? ShowStatus { get; set; }

    public string? ShowAction { get; set; }

    public string? ShowDirection { get; set; }

    public string? PrivateShowId { get; set; }

    public string? PrivateShowSessionId { get; set; }

    public string? PrivateShowTicketId { get; set; }

    public string? GroupShowId { get; set; }

    public string? GroupShowSessionId { get; set; }

    public string? GroupShowTicketId { get; set; }

    public string? VoiceShowId { get; set; }

    public string? VoiceShowSessionId { get; set; }

    public string? VoiceShowTicketId { get; set; }

    public string? VideoShowId { get; set; }

    public string? VideoShowSessionId { get; set; }

    public string? VideoShowTicketId { get; set; }

    public string? GroupShowInviteId { get; set; }

    public string? GroupShowInviteType { get; set; }

    public string? GroupShowStartMessageId { get; set; }

    public int? GroupShowEndMessageId { get; set; }

    public bool? IsNotificationShow { get; set; }

    public DateTime? NotificationStart { get; set; }

    public DateTime? NotificationReset { get; set; }

    public string? EventSoundCode { get; set; }

    public string? EventSoundText { get; set; }

    public string? EventSoundValue { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerSessionId { get; set; }

    public string? InviteOwnerType { get; set; }

    public string? InviteOwnerRole { get; set; }

    public string? FromUserId { get; set; }

    public string? FromTlknUserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToTlknUserId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? ToGroupId { get; set; }

    public string? ToGroupUserList { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsScreenOnline { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
