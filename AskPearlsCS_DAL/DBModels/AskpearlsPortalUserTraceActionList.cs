using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserTraceActionList
{
    public long TuraceActionId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? TokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? OwnerUserId { get; set; }

    public string? OwnerMobileNumberId { get; set; }

    public string? RemoteUserId { get; set; }

    public string? RemoteMobileNumberId { get; set; }

    public string? SessionId { get; set; }

    public string? TraceTokenId { get; set; }

    public string? TraceTarget { get; set; }

    public string? TraceDirection { get; set; }

    public string? TraceAction { get; set; }

    public string? TraceCode { get; set; }

    public string? TraceType { get; set; }

    public string? TraceValue { get; set; }

    public string? MessageCode { get; set; }

    public string? MessageType { get; set; }

    public string? MessageText { get; set; }

    public int? TraceUpdateDay { get; set; }

    public int? TraceUpdateMonth { get; set; }

    public int? TraceUpdateYear { get; set; }

    public DateTime? TraceUpdateDate { get; set; }

    public string? Username { get; set; }

    public string? GuestId { get; set; }

    public string? GuestName { get; set; }

    public string? GuestNameLower { get; set; }

    public string? UserType { get; set; }

    public string? UserStatus { get; set; }

    public bool? IsOnline { get; set; }

    public string? UserIp { get; set; }

    public string? SectionName { get; set; }

    public string? PageName { get; set; }

    public string? PageTitle { get; set; }

    public string? PageDscription { get; set; }

    public string? PageUrl { get; set; }

    public string? LoginDay { get; set; }

    public string? LoginMonth { get; set; }

    public string? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public string? LogoutDay { get; set; }

    public string? LogoutMonth { get; set; }

    public string? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsVisibleOwner { get; set; }

    public bool? IsVisibleRemote { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
