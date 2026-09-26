using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowLoginList
{
    public long UscreenChatShowLoginId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenChatShowTicketTempId { get; set; }

    public string? GroupTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? ApplicationRoleTokenId { get; set; }

    public string? ApplicationRoleCode { get; set; }

    public string? ProjectRoleTokenId { get; set; }

    public string? ProjectRoleCode { get; set; }

    public string? GroupRoleTokenId { get; set; }

    public string? GroupRoleCode { get; set; }

    public string? RemoteUserTokenId { get; set; }

    public string? RemoteMobileNumberTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? TrackuserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? ScreenUserType { get; set; }

    public string? ScreenChatShowStatus { get; set; }

    public string? UserType { get; set; }

    public string? ShowId { get; set; }

    public string? ShowSessionId { get; set; }

    public string? ShowTicketId { get; set; }

    public string? ShowCode { get; set; }

    public string? ShowType { get; set; }

    public string? ShowStatus { get; set; }

    public string? ShowAction { get; set; }

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

    public bool? IsNotificationShow { get; set; }

    public DateTime? NotificationStart { get; set; }

    public DateTime? NotificationReset { get; set; }

    public string? EventSoundCode { get; set; }

    public string? EventSoundText { get; set; }

    public string? EventSoundValue { get; set; }

    public string? FromUserId { get; set; }

    public string? FromTrackuserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToTrackuserId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsOnline { get; set; }

    public bool? IsLogin { get; set; }

    public bool? IsScreenOnline { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
