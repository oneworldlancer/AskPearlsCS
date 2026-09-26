using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowInvitationList
{
    public long ScreenChatShowInviteId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenChatShowTicketTempId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? VideoShowId { get; set; }

    public string? VideoShowSessionId { get; set; }

    public string? ScreenChatShowType { get; set; }

    public string? ScreenUserType { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerMobileNumberId { get; set; }

    public string? InviteOwnerUserSessionId { get; set; }

    public string? InviteOwnerUserType { get; set; }

    public string? InviteOwnerUserRole { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? TlknUserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? FromTlknUserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ToTlknUserId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? ScreenChatShowStatus { get; set; }

    public bool? IsScreenChatShowInviteOnline { get; set; }

    public bool? IsScreenChatShowInviteShow { get; set; }

    public DateTime? ScreenChatShowInviteStart { get; set; }

    public DateTime? ScreenChatShowInviteReset { get; set; }

    public DateTime? ScreenChatShowOpenTime { get; set; }

    public DateTime? ScreenChatShowCloseTime { get; set; }

    public DateTime? ScreenChatShowInitiateTime { get; set; }

    public DateTime? ScreenChatShowPendingTime { get; set; }

    public DateTime? ScreenChatShowStartTime { get; set; }

    public DateTime? ScreenChatShowEndTime { get; set; }

    public DateTime? ScreenChatShowSpendTime { get; set; }

    public bool? IsScreenChatShowOpen { get; set; }

    public bool? IsScreenChatShowClose { get; set; }

    public bool? IsScreenChatShowInitiate { get; set; }

    public bool? IsScreenChatShowPending { get; set; }

    public bool? IsScreenChatShowStart { get; set; }

    public bool? IsScreenChatShowEnd { get; set; }

    public bool? IsScreenChatShowSpend { get; set; }

    public int? ScreenChatShowTimeSpendHour { get; set; }

    public int? ScreenChatShowTimeSpendMinute { get; set; }

    public int? ScreenChatShowTimeSpendSecond { get; set; }

    public int? TimeStartDay { get; set; }

    public int? TimeStartMonth { get; set; }

    public int? TimeStartYear { get; set; }

    public DateTime? TimeStartDate { get; set; }

    public int? TimeEndDay { get; set; }

    public int? TimeEndMonth { get; set; }

    public int? TimeEndYear { get; set; }

    public DateTime? TimeEndDate { get; set; }

    public int? LoginDay { get; set; }

    public int? LoginMonth { get; set; }

    public int? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public int? LogoutDay { get; set; }

    public int? LogoutMonth { get; set; }

    public int? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsScreenOnline { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
