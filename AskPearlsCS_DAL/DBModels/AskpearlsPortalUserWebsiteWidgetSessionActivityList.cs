using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserWebsiteWidgetSessionActivityList
{
    public long UwebsiteWidgetSessionActivityId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? PlatformOsid { get; set; }

    public string? AccountType { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? CompanyName { get; set; }

    public string? SessionTokenId { get; set; }

    public string? UserSessionTokenId { get; set; }

    public string? WidgetSessionTokenId { get; set; }

    public string? WidgetActivityTokenId { get; set; }

    public string? WidgetSessionActivityTokenId { get; set; }

    public string? WidgetSessionStatus { get; set; }

    public string? WebsiteTokenId { get; set; }

    public string? WidgetTokenId { get; set; }

    public string? WidgetVersionTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? LastActivityAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string? DurationSeconds { get; set; }

    public string? QuestionCount { get; set; }

    public string? AnsweredQuestionCount { get; set; }

    public string? RecommendationTokenId { get; set; }

    public string? RecommendationScore { get; set; }

    public string? RecommendationPercentage { get; set; }

    public string? PageUrl { get; set; }

    public string? ReferrerUrl { get; set; }

    public string? UserAgent { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent2 { get; set; }

    public string? UserAgent1 { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? GuestId { get; set; }

    public string? GuestGuidtoknId { get; set; }

    public string? GuestUserSessionTokenId { get; set; }

    public string? GuestUserTokenId { get; set; }

    public string? GuestMobileNumberTokenId { get; set; }

    public string? UserBrowserCode { get; set; }

    public string? UserBrowserType { get; set; }

    public string? UserBrowserName { get; set; }

    public string? UserBrowserTitle { get; set; }

    public string? UserBrowserVersion { get; set; }

    public string? UserBrowserInfo { get; set; }

    public string? UserCountryCode { get; set; }

    public string? UserCountryName { get; set; }

    public string? UserProvince { get; set; }

    public string? UserState { get; set; }

    public string? UserCity { get; set; }

    public DateTime? UserDateTimeNow { get; set; }

    public string? UserIp { get; set; }

    public string? UserType { get; set; }

    public bool? IsGuest { get; set; }

    public string? LoginLocationCountryName { get; set; }

    public string? LoginLocationCountryCode { get; set; }

    public string? LoginLocationRegionName { get; set; }

    public string? LoginUserIp { get; set; }

    public string? UserLoginStatus { get; set; }

    public string? UserMoodStatus { get; set; }

    public string? LoginDay { get; set; }

    public string? LoginMonth { get; set; }

    public string? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public string? LogoutDay { get; set; }

    public string? LogoutMonth { get; set; }

    public string? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVisible { get; set; }

    public DateTime? CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
