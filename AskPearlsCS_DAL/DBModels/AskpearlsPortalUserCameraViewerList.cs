using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserCameraViewerList
{
    public long UcamId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CamId { get; set; }

    public string? CamFmsid { get; set; }

    public string? CamFmsurl { get; set; }

    public string? CamCode { get; set; }

    public string? FromUserType { get; set; }

    public string? ToUserType { get; set; }

    public string? FromUserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? CamStatus { get; set; }

    public bool? IsOnline { get; set; }

    public bool? IsOpen { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsNewBoundFromUser { get; set; }

    public bool? IsNewBoundToUser { get; set; }

    public bool? IsModelPmchat { get; set; }

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
