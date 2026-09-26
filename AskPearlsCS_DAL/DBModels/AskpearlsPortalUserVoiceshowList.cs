using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVoiceshowList
{
    public long VoiceShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? VoiceShowSessionId { get; set; }

    public string? VoiceShowype { get; set; }

    public string? UserId { get; set; }

    public string? TlknUserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

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

    public string? VoiceShowStatus { get; set; }

    public bool? IsVoiceShowLoad { get; set; }

    public DateTime? VoiceShowOpenTime { get; set; }

    public DateTime? VoiceShowCloseTime { get; set; }

    public DateTime? VoiceShowInitiateTime { get; set; }

    public DateTime? VoiceShowPendingTime { get; set; }

    public DateTime? VoiceShowStartTime { get; set; }

    public DateTime? VoiceShowEndTime { get; set; }

    public DateTime? VoiceShowSpendTime { get; set; }

    public bool? IsVoiceShowOpen { get; set; }

    public bool? IsVoiceShowClose { get; set; }

    public bool? IsVoiceShowInitiate { get; set; }

    public bool? IsVoiceShowPending { get; set; }

    public bool? IsVoiceShowStart { get; set; }

    public bool? IsVoiceShowEnd { get; set; }

    public bool? IsVoiceShowSpend { get; set; }

    public int? VoiceShowTimeSpendHour { get; set; }

    public int? VoiceShowTimeSpendMinute { get; set; }

    public int? VoiceShowTimeSpendSecond { get; set; }

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

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
