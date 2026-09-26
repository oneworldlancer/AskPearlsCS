using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserDatingMatchEventList
{
    public long EventSoundId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FriendId { get; set; }

    public string? FriendMobileNumberId { get; set; }

    public string? MatchLikeStatus { get; set; }

    public string? UserType { get; set; }

    public DateTime? EventTimeStart { get; set; }

    public DateTime? EventTimeEnd { get; set; }

    public DateTime? EventTimePending { get; set; }

    public DateTime? EventTimeReset { get; set; }

    public string? EventSoundText { get; set; }

    public string? EventSoundValue { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? EventDateTimeStart { get; set; }

    public DateTime? EventDateTimeEnd { get; set; }

    public DateTime? EventDateTimeReset { get; set; }

    public string? UploadDateTimeMilliSecStart { get; set; }

    public string? UploadDateTimeMilliSecEnd { get; set; }

    public string? UploadDateTimeMilliSecReset { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
