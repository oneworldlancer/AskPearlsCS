using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGroupshowInvitationList
{
    public long GroupShowInviteId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? GroupShowId { get; set; }

    public string? GroupShowTicketId { get; set; }

    public string? GroupShowSessionId { get; set; }

    public string? GroupShowType { get; set; }

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

    public string? GroupShowStatus { get; set; }

    public string? GroupShowInviteType { get; set; }

    public bool? IsGroupShowInviteOnline { get; set; }

    public bool? IsGroupShowInviteShow { get; set; }

    public DateTime? GroupShowInviteStart { get; set; }

    public DateTime? GroupShowInviteReset { get; set; }

    public DateTime? GroupShowOpenTime { get; set; }

    public DateTime? GroupShowCloseTime { get; set; }

    public DateTime? GroupShowInitiateTime { get; set; }

    public DateTime? GroupShowPendingTime { get; set; }

    public DateTime? GroupShowStartTime { get; set; }

    public DateTime? GroupShowEndTime { get; set; }

    public DateTime? GroupShowSpendTime { get; set; }

    public bool? IsGroupShowOpen { get; set; }

    public bool? IsGroupShowClose { get; set; }

    public bool? IsGroupShowInitiate { get; set; }

    public bool? IsGroupShowPending { get; set; }

    public bool? IsGroupShowStart { get; set; }

    public bool? IsGroupShowEnd { get; set; }

    public bool? IsGroupShowSpend { get; set; }

    public int? GroupShowTimeSpendHour { get; set; }

    public int? GroupShowTimeSpendMinute { get; set; }

    public int? GroupShowTimeSpendSecond { get; set; }

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

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
