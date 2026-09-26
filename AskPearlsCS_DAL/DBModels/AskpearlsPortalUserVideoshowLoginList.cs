using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVideoshowLoginList
{
    public long LoginVideoShowroomId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? VideoShowTicketId { get; set; }

    public string? VideoShowId { get; set; }

    public string? VideoShowInviteId { get; set; }

    public string? VideoShowSessionId { get; set; }

    public string? VideoShowInviteType { get; set; }

    public string? UserId { get; set; }

    public string? Username { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? UserInviteRole { get; set; }

    public bool? IsGuest { get; set; }

    public string? UserStatus { get; set; }

    public int? UploadMonth { get; set; }

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

    public int? UploadDay { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
