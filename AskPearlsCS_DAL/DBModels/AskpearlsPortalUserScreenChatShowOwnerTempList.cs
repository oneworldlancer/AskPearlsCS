using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowOwnerTempList
{
    public long OwnerTempId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? InviteTag { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenChatShowTicketTempId { get; set; }

    public string? ScreenChatShowTicketLineId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? TlknUserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? ScreenUserType { get; set; }

    public string? UserType { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerSessionId { get; set; }

    public string? InviteOwnerType { get; set; }

    public string? InviteOwnerRole { get; set; }

    public string? InviteOwnerRequestUserId { get; set; }

    public string? InviteOwnerRequestType { get; set; }

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

    public bool? IsAdmin { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
