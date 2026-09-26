using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserVideoshowLiveList
{
    public long VideoShowLiveId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? VideoShowId { get; set; }

    public string? VideoShowSessionId { get; set; }

    public string? UserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? VideoShowStatus { get; set; }

    public DateTime? VideoShowStartTime { get; set; }

    public DateTime? VideoShowEndTime { get; set; }

    public DateTime? VideoShowOpenTime { get; set; }

    public DateTime? VideoShowCloseTime { get; set; }

    public DateTime? VideoShowInitiateTime { get; set; }

    public DateTime? VideoShowPendingTime { get; set; }

    public DateTime? VideoShowSpendTime { get; set; }

    public bool? IsVideoShowOpen { get; set; }

    public bool? IsVideoShowClose { get; set; }

    public bool? IsVideoShowInitiate { get; set; }

    public bool? IsVideoShowPending { get; set; }

    public bool? IsVideoShowStart { get; set; }

    public bool? IsVideoShowEnd { get; set; }

    public bool? IsVideoShowSpend { get; set; }

    public int? VideoShowTimeSpendHour { get; set; }

    public int? VideoShowTimeSpendMinute { get; set; }

    public int? VideoShowTimeSpendSecond { get; set; }

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
