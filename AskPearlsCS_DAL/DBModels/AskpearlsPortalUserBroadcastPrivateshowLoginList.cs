using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastPrivateshowLoginList
{
    public long UprivateShowLoginId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? PrivateShowId { get; set; }

    public string? PrivateShowTicketId { get; set; }

    public string? PrivateShowTokenId { get; set; }

    public string? PrivateShowVideoId { get; set; }

    public string? PrivateShowVideoTokenId { get; set; }

    public string? PrivateShowInviteId { get; set; }

    public string? PrivateShowSessionId { get; set; }

    public string? PrivateShowInviteType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? Username { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? UserInviteRole { get; set; }

    public bool? IsGuest { get; set; }

    public string? PrivateShowStatus { get; set; }

    public string? UserStatus { get; set; }

    public bool? IsLogin { get; set; }

    public bool? IsOnline { get; set; }

    public int? UserOrder { get; set; }

    public DateTime? UserRegisterDate { get; set; }

    public DateTime? ResetDate { get; set; }

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
