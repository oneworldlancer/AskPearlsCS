using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastPrivateshowCameraEventList
{
    public long UprivateShowCameraEventId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public string? PrivateShowId { get; set; }

    public string? PrivateShowTicketId { get; set; }

    public string? PrivateShowTokenId { get; set; }

    public string? PrivateShowVideoId { get; set; }

    public string? PrivateShowVideoTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CamFmsid { get; set; }

    public string? CamFmsurl { get; set; }

    public string? CameraEventCode { get; set; }

    public string? TipSpendCode { get; set; }

    public string? PrivateShowInviteId { get; set; }

    public string? PrivateShowSessionId { get; set; }

    public int? StudioId { get; set; }

    public bool? IsModelStudio { get; set; }

    public int? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public int? FromUserId { get; set; }

    public int? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? FromUserInviteRole { get; set; }

    public int? ToUserId { get; set; }

    public int? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

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

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
