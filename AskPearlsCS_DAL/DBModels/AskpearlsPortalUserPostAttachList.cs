using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPostAttachList
{
    public long UattachId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? PostId { get; set; }

    public string? PostTokenId { get; set; }

    public string? UpostTokenId { get; set; }

    public string? FileCode { get; set; }

    public string? FileType { get; set; }

    public string? FileId { get; set; }

    public string? FileThumUrl { get; set; }

    public string? FileUrl { get; set; }

    public string? FileDuration { get; set; }

    public string? FileDurationValue { get; set; }

    public string? FileDurationText { get; set; }

    public string? YouTubeChannelId { get; set; }

    public string? FileName { get; set; }

    public string? FileTitle { get; set; }

    public string? FileDescription { get; set; }

    public string? MessageTextCss { get; set; }

    public string? MessageText { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? StateName { get; set; }

    public string? CityName { get; set; }

    public string? AddressLine { get; set; }

    public bool? IsViewOnly { get; set; }

    public string? AttachTag { get; set; }

    public string? PostTag { get; set; }

    public string? PostFrom { get; set; }

    public string? PostLine { get; set; }

    public string? PostDirection { get; set; }

    public string? GroupId { get; set; }

    public string? FriendId { get; set; }

    public string? FriendMobileNumberId { get; set; }

    public string? OwnerUserId { get; set; }

    public string? OwnerMobileNumberId { get; set; }

    public string? RemoteUserId { get; set; }

    public string? RemoteMobileNumberId { get; set; }

    public string? UserType { get; set; }

    public DateTime? DateAdd { get; set; }

    public DateTime? DateRemove { get; set; }

    public string? BlackListStatus { get; set; }

    public bool? IsBlackList { get; set; }

    public string? MatchLikeStatus { get; set; }

    public bool? IsMatch { get; set; }

    public bool? IsDatingMatchConnect { get; set; }

    public bool? IsLike { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsUserLike { get; set; }

    public bool? IsUserShare { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsView { get; set; }

    public bool? IsActive { get; set; }

    public int? RemoveDay { get; set; }

    public int? RemoveMonth { get; set; }

    public int? RemoveYear { get; set; }

    public DateTime? RemoveDate { get; set; }

    public string? UploadTime { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public DateTime? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
