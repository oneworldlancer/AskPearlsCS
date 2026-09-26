using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastPrivateshowInvitationRequestList
{
    public long UprivateShowInviteId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? PrivateShowHandlerTokenId { get; set; }

    public string? PrivateShowId { get; set; }

    public string? PrivateShowTicketId { get; set; }

    public string? PrivateShowTokenId { get; set; }

    public string? PrivateShowVideoId { get; set; }

    public string? PrivateShowVideoTokenId { get; set; }

    public string? PrivateShowSessionId { get; set; }

    public string? PrivateShowType { get; set; }

    public string? PrivateShowQueueOrder { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? TlknUserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public int? FromUserId { get; set; }

    public int? FromMobileNumberId { get; set; }

    public int? FromTlknUserId { get; set; }

    public int? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public int? ToUserId { get; set; }

    public int? ToMobileNumberId { get; set; }

    public int? ToTlknUserId { get; set; }

    public int? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? PrivateShowStatus { get; set; }

    public string? PrivateShowStatusOwner { get; set; }

    public string? PrivateShowStatusRemote { get; set; }

    public bool? IsOpen { get; set; }

    public string? PrivateShowInviteType { get; set; }

    public bool? IsPrivateShowInviteOnline { get; set; }

    public bool? IsPrivateShowInviteShow { get; set; }

    public DateTime? PrivateShowInviteStart { get; set; }

    public DateTime? PrivateShowInviteReset { get; set; }

    public DateTime? PrivateShowOpenTime { get; set; }

    public DateTime? PrivateShowCloseTime { get; set; }

    public DateTime? PrivateShowInitiateTime { get; set; }

    public DateTime? PrivateShowPendingTime { get; set; }

    public DateTime? PrivateShowStartTime { get; set; }

    public DateTime? PrivateShowEndTime { get; set; }

    public DateTime? PrivateShowSpendTime { get; set; }

    public bool? IsPrivateShowOpen { get; set; }

    public bool? IsPrivateShowClose { get; set; }

    public bool? IsPrivateShowInitiate { get; set; }

    public bool? IsPrivateShowPending { get; set; }

    public bool? IsPrivateShowStart { get; set; }

    public bool? IsPrivateShowEnd { get; set; }

    public bool? IsPrivateShowSpend { get; set; }

    public int? PrivateShowTimeSpendHour { get; set; }

    public int? PrivateShowTimeSpendMinute { get; set; }

    public int? PrivateShowTimeSpendSecond { get; set; }

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

    public bool? IsViewOwner { get; set; }

    public bool? IsViewRemote { get; set; }

    public bool? IsNewOwner { get; set; }

    public bool? IsNewRemote { get; set; }

    public bool? IsVisibleOwner { get; set; }

    public bool? IsVisibleRemote { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
