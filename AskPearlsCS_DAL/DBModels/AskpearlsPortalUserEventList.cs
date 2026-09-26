using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserEventList
{
    public long EventId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? EventTokenId { get; set; }

    public string? EventCategoryId { get; set; }

    public string? EventCategoryCode { get; set; }

    public string? EventCategoryType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? EventInviteStatusOwner { get; set; }

    public string? EventCode { get; set; }

    public string? EventType { get; set; }

    public string? EventName { get; set; }

    public string? EventTitle { get; set; }

    public string? EventDescription { get; set; }

    public string? EventColor { get; set; }

    public int? EventOrder { get; set; }

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

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? StateName { get; set; }

    public string? CityName { get; set; }

    public string? AddressLine { get; set; }

    public string? EventStaticMapUrl { get; set; }

    public int? EventImageId { get; set; }

    public string? EventImageUrl { get; set; }

    public bool? IsEventPublic { get; set; }

    public string? StreetLine { get; set; }

    public string? EventHour { get; set; }

    public string? EventMinute { get; set; }

    public string? EventTimeZone { get; set; }

    public string? EventGmtzone { get; set; }

    public string? EventStartHour { get; set; }

    public string? EventStartHourDay { get; set; }

    public string? EventStartMinute { get; set; }

    public string? EventStartTimeZone { get; set; }

    public string? EventStartGmtzone { get; set; }

    public string? EventEndHour { get; set; }

    public string? EventEndHourDay { get; set; }

    public string? EventEndMinute { get; set; }

    public string? EventEndTimeZone { get; set; }

    public string? EventEndGmtzone { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public byte[]? ImageData { get; set; }

    public string? UserType { get; set; }

    public DateTime? DateAdd { get; set; }

    public DateTime? DateRemove { get; set; }

    public bool? IsEventAllDay { get; set; }

    public bool? IsEventRepeat { get; set; }

    public string? EventRepeatCode { get; set; }

    public bool? IsEventReminder { get; set; }

    public string? EventReminderSetTime { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public virtual ICollection<AskpearlsPortalUserEventInvitationList> AskpearlsPortalUserEventInvitationLists { get; set; } = new List<AskpearlsPortalUserEventInvitationList>();
}
