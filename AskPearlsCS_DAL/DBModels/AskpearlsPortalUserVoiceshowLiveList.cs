using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVoiceshowLiveList
{
    public long VoiceshowliveId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? Voiceshowid { get; set; }

    public string? VoiceshowsessionId { get; set; }

    public string? UserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? Voiceshowstatus { get; set; }

    public DateTime? VoiceshowstartTime { get; set; }

    public DateTime? VoiceshowendTime { get; set; }

    public DateTime? VoiceshowopenTime { get; set; }

    public DateTime? VoiceshowcloseTime { get; set; }

    public DateTime? VoiceshowinitiateTime { get; set; }

    public DateTime? VoiceshowpendingTime { get; set; }

    public DateTime? VoiceshowspendTime { get; set; }

    public bool? IsVoiceshowopen { get; set; }

    public bool? IsVoiceshowclose { get; set; }

    public bool? IsVoiceshowinitiate { get; set; }

    public bool? IsVoiceshowpending { get; set; }

    public bool? IsVoiceshowstart { get; set; }

    public bool? IsVoiceshowend { get; set; }

    public bool? IsVoiceshowspend { get; set; }

    public int? VoiceshowtimeSpendHour { get; set; }

    public int? VoiceshowtimeSpendMinute { get; set; }

    public int? VoiceshowtimeSpendSecond { get; set; }

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
