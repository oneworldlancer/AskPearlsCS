using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVideoshowMessageList
{
    public long MsgVideoShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? VideoShowTicketId { get; set; }

    public string? CamFmsurl { get; set; }

    public string? VideoShowInviteId { get; set; }

    public string? VideoShowId { get; set; }

    public string? VideoShowSessionId { get; set; }

    public string? UserId { get; set; }

    public string? Username { get; set; }

    public string? SessionId { get; set; }

    public string? UserType { get; set; }

    public string? MessageText { get; set; }

    public bool? IsMessageOnline { get; set; }

    public bool? IsGuest { get; set; }

    public bool? IsGuestMute { get; set; }

    public bool? IsUserBlacklist { get; set; }

    public bool? IsUserStudio { get; set; }

    public bool? IsUserAdmin { get; set; }

    public int? UserLoginId { get; set; }

    public int? UserLoginSessionId { get; set; }

    public int? GuestLoginId { get; set; }

    public int? GuestLoginSessionId { get; set; }

    public bool? IsTopic { get; set; }

    public bool? IsToken { get; set; }

    public bool? IsRead { get; set; }

    public bool? IsTokenVisible { get; set; }

    public DateTime? ResetDate { get; set; }

    public string? LoginDay { get; set; }

    public string? LoginMonth { get; set; }

    public string? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public string? LogoutDay { get; set; }

    public string? LogoutMonth { get; set; }

    public string? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
