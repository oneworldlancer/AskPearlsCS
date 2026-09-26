using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserEventInvitationList
{
    public long UeventInviteId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? EventInviteTokenId { get; set; }

    public long? EventId { get; set; }

    public string? EventTokenId { get; set; }

    public string? EventInviteCategory { get; set; }

    public string? EventInviteCode { get; set; }

    public string? EventInviteType { get; set; }

    public string? EventInviteTarget { get; set; }

    public string? EventInviteTag { get; set; }

    public string? EventInviteUserType { get; set; }

    public string? EventInviteUrl { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? GroupId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? FromTlknUserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ToUsername { get; set; }

    public string? ToMobileNumber { get; set; }

    public string? ToEmailAddress { get; set; }

    public int? EventDay { get; set; }

    public int? EventMonth { get; set; }

    public int? EventYear { get; set; }

    public string? EventDateTimeMilliSec { get; set; }

    public string? EventDateStringFormat { get; set; }

    public string? EventTimeStringFormat { get; set; }

    public int? EventStartDay { get; set; }

    public int? EventStartMonth { get; set; }

    public int? EventStartYear { get; set; }

    public string? EventStartDateTimeMilliSec { get; set; }

    public string? EventStartDateStringFormat { get; set; }

    public string? EventStartTimeStringFormat { get; set; }

    public int? EventEndDay { get; set; }

    public int? EventEndMonth { get; set; }

    public int? EventEndYear { get; set; }

    public string? EventEndDateTimeMilliSec { get; set; }

    public string? EventEndDateStringFormat { get; set; }

    public string? EventEndTimeStringFormat { get; set; }

    public string? EventHour { get; set; }

    public string? EventMinute { get; set; }

    public string? EventTimeZone { get; set; }

    public string? EventGmtzone { get; set; }

    public string? EventStartHour { get; set; }

    public string? EventStartMinute { get; set; }

    public string? EventStartTimeZone { get; set; }

    public string? EventStartGmtzone { get; set; }

    public string? EventEndHour { get; set; }

    public string? EventEndMinute { get; set; }

    public string? EventEndTimeZone { get; set; }

    public string? EventEndGmtzone { get; set; }

    public int? ToTlknUserId { get; set; }

    public int? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? StartDateTimeMilliSec { get; set; }

    public bool? IsOpen { get; set; }

    public string? EventInviteStatus { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsVisibleOwner { get; set; }

    public bool? IsVisibleRemote { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public virtual AskpearlsPortalUserEventList? Event { get; set; }
}
