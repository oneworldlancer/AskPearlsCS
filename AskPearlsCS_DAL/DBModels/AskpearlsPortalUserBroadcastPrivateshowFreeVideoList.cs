using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBroadcastPrivateshowFreeVideoList
{
    public long UprivateShowFreeVideoId { get; set; }

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

    public string? PrivateShowSessionId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? PrivateShowStatus { get; set; }

    public int? TodayDay { get; set; }

    public int? TodayMonth { get; set; }

    public int? TodayYear { get; set; }

    public int? TimeCountStart { get; set; }

    public int? TimeCountEnd { get; set; }

    public int? TimeCountConsume { get; set; }

    public int? TimeCountFreeLeft { get; set; }

    public int? TimeCountFreeTotal { get; set; }

    public int? TimeCountLeft1 { get; set; }

    public int? TimeCountTotal1 { get; set; }

    public DateTime? PrivateShowStartTime { get; set; }

    public DateTime? PrivateShowEndTime { get; set; }

    public DateTime? PrivateShowOpenTime { get; set; }

    public DateTime? PrivateShowCloseTime { get; set; }

    public DateTime? PrivateShowInitiateTime { get; set; }

    public DateTime? PrivateShowPendingTime { get; set; }

    public DateTime? PrivateShowSpendTime { get; set; }

    public bool? IsPrivateShowOpen { get; set; }

    public bool? IsPrivateShowClose { get; set; }

    public bool? IsPrivateShowInitiate { get; set; }

    public bool? IsPrivateShowPending { get; set; }

    public bool? IsPrivateShowStart { get; set; }

    public bool? IsPrivateShowEnd { get; set; }

    public bool? IsPrivateShowSpend { get; set; }

    public int? PrivateShowTimeSpendHour { get; set; }

    public int? PrivateShowTimeSpendMinute { get; set; }

    public int? PrivateShowTimeSpendSecond { get; set; }

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

    public bool? IsOpen { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
