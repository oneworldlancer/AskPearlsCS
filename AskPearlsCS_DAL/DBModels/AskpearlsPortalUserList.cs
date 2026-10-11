using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserList
{
    public long UserId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? Guid { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MembershipType { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? Gcmid { get; set; }

    public string? UserGcmid { get; set; }

    public string? MobileDeviceType { get; set; }

    public string? MobileDeviceNumber { get; set; }

    public string? SystemName { get; set; }

    public string? Gender { get; set; }

    public string? UsernameText { get; set; }

    public string? UsernameTextLower { get; set; }

    public string? Username { get; set; }

    public string? UsernameLower { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? FirstNameLower { get; set; }

    public string? LastName { get; set; }

    public string? LastNameLower { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? BirthDateDay { get; set; }

    public int? BirthDateMonth { get; set; }

    public int? BirthDateYear { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public string? EmailAddress { get; set; }

    public bool? IsEmailVerify { get; set; }

    public int? CountryId { get; set; }

    public int? CityId { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? CountryName { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryFlagUrl { get; set; }

    public string? CityName { get; set; }

    public string? Address { get; set; }

    public string? Street { get; set; }

    public string? PostalCode { get; set; }

    public bool? IsValidateMobileNumber { get; set; }

    public bool? IsValidateEmailAddress { get; set; }

    public bool? IsFirstLogin { get; set; }

    public bool? IsAutoLogin { get; set; }

    public bool? IsRemeberMe { get; set; }

    public string? Notes { get; set; }

    public string? UserRole { get; set; }

    public string? LoginType { get; set; }

    public string? OnlineStatus { get; set; }

    public string? LoginStatus { get; set; }

    public string? VisibleStatus { get; set; }

    public string? UserLoginStatus { get; set; }

    public string? UserMoodStatus { get; set; }

    public string? UserNotificationKeyId { get; set; }

    public string? UserNotificationKeyName { get; set; }

    public bool? IsDeviceOnline { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? ActiveViewed { get; set; }

    public bool? IsWebmaster { get; set; }

    public bool? IsModerator { get; set; }

    public bool? IsLike { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsSystemUser { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVisible { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public virtual ICollection<AskpearlsPortalUserForumCommentReplyEventList> AskpearlsPortalUserForumCommentReplyEventLists { get; set; } = new List<AskpearlsPortalUserForumCommentReplyEventList>();
}
