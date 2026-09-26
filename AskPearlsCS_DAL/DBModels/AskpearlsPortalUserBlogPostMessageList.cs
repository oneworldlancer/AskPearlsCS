using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBlogPostMessageList
{
    public long UblogPostId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PostTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FriendId { get; set; }

    public string? FriendMobileNumberId { get; set; }

    public string? OwnerUserId { get; set; }

    public string? OwnerMobileNumberId { get; set; }

    public string? RemoteUserId { get; set; }

    public string? RemoteMobileNumberId { get; set; }

    public string? UserType { get; set; }

    public string? PostCode { get; set; }

    public string? PostType { get; set; }

    public string? PostCss { get; set; }

    public string? PostNameCss { get; set; }

    public string? PostTitleCss { get; set; }

    public string? PostDescriptionCss { get; set; }

    public string? PostMiniDescriptionCss { get; set; }

    public string? PostFullDescriptionCss { get; set; }

    public string? PostLineCss { get; set; }

    public string? PostTextCss { get; set; }

    public string? PostMessageTextCss { get; set; }

    public string? PostUrl { get; set; }

    public string? PostName { get; set; }

    public string? PostTitle { get; set; }

    public string? PostDescription { get; set; }

    public string? PostMiniDescription { get; set; }

    public string? PostFullDescription { get; set; }

    public string? PostTag { get; set; }

    public string? PostLine { get; set; }

    public string? PostText { get; set; }

    public string? PostMessageText { get; set; }

    public string? MessageName { get; set; }

    public string? MessageTitle { get; set; }

    public string? MessageDescription { get; set; }

    public string? MessageMiniDescription { get; set; }

    public string? MessageFullDescription { get; set; }

    public string? MessageLine { get; set; }

    public string? MessageText { get; set; }

    public string? FileId { get; set; }

    public string? FileCode { get; set; }

    public string? FileType { get; set; }

    public string? FileUrl { get; set; }

    public string? FileServerUrl { get; set; }

    public bool? FileIsViewOnly { get; set; }

    public string? FileState { get; set; }

    public string? ShareFileCode { get; set; }

    public string? ShareFileType { get; set; }

    public bool? IsNewMessage { get; set; }

    public string? ImageDefaultServerId { get; set; }

    public string? ImageDefaultServerUrl { get; set; }

    public string? ImageDefaultPath { get; set; }

    public string? ImageDefaultType { get; set; }

    public string? ImageDefaultData { get; set; }

    public string? ImageThumServerUrl { get; set; }

    public string? ImageThumPath { get; set; }

    public string? ImageThumData { get; set; }

    public bool? ImageIsLoading { get; set; }

    public bool? ImageIsDownload { get; set; }

    public bool? ImageIsSaved { get; set; }

    public bool? ImageIsViewOnly { get; set; }

    public int? ImageFromUserId { get; set; }

    public int? ImageFromMobileNumberId { get; set; }

    public int? ImageToUserId { get; set; }

    public int? ImageToMobileNumberId { get; set; }

    public string? VideoDefaultServerId { get; set; }

    public string? VideoDefaultServerUrl { get; set; }

    public string? VideoDefaultPath { get; set; }

    public string? VideoDefaultType { get; set; }

    public string? VideoDefaultData { get; set; }

    public string? VideoThumServerUrl { get; set; }

    public string? VideoThumPath { get; set; }

    public string? VideoThumData { get; set; }

    public bool? VideoIsLoading { get; set; }

    public bool? VideoIsDownload { get; set; }

    public bool? VideoIsSaved { get; set; }

    public bool? VideoIsViewOnly { get; set; }

    public int? VideoFromUserId { get; set; }

    public int? VideoFromMobileNumberId { get; set; }

    public int? VideoToUserId { get; set; }

    public int? VideoToMobileNumberId { get; set; }

    public int? LocationRequestFromRemoteUserId { get; set; }

    public int? LocationRequestToRemoteUserId { get; set; }

    public string? LocationType { get; set; }

    public string? LocationLink { get; set; }

    public string? LocationLongitude { get; set; }

    public string? LocationLatitude { get; set; }

    public string? LocationCountryName { get; set; }

    public string? LocationCountryCode { get; set; }

    public string? LocationCountryFlagUrl { get; set; }

    public string? LocationCityName { get; set; }

    public string? LocationAddress { get; set; }

    public string? LocationStreet { get; set; }

    public int? SuggestContactListFromRemoteUserId { get; set; }

    public int? SuggestContactListToRemoteUserId { get; set; }

    public string? SuggestContactListUserId { get; set; }

    public string? SuggestContactListMobileNumberId { get; set; }

    public bool? IsShareLocationAskRemoteEnable { get; set; }

    public bool? IsSubjectLine { get; set; }

    public bool? IsHistoryLine { get; set; }

    public bool? IsSpaceLine { get; set; }

    public bool? IsSessionClosedLine { get; set; }

    public bool? IsCreateNewChatLine { get; set; }

    public DateTime? DateAdd { get; set; }

    public DateTime? DateRemove { get; set; }

    public string? BlackListStatus { get; set; }

    public bool? IsBlackList { get; set; }

    public string? MatchLikeStatus { get; set; }

    public bool? IsMatch { get; set; }

    public bool? IsDatingMatchConnect { get; set; }

    public bool? IsPostShare { get; set; }

    public bool? IsPostView { get; set; }

    public bool? IsPostSave { get; set; }

    public bool? IsLike { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public int? RemoveDay { get; set; }

    public int? RemoveMonth { get; set; }

    public int? RemoveYear { get; set; }

    public DateTime? RemoveDate { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
