using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGroupshowMessageList
{
    public long MsgGroupShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public bool? IsDating { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerMobileNumberId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? GroupId { get; set; }

    public string? GroupType { get; set; }

    public string? VoiceShowInviteId { get; set; }

    public string? VideoShowInviteId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserName { get; set; }

    public string? VisibleStatus { get; set; }

    public string? InviteCode { get; set; }

    public string? InviteType { get; set; }

    public string? MessageType { get; set; }

    public string? MessageText { get; set; }

    public string? FileId { get; set; }

    public string? FileCode { get; set; }

    public string? FileType { get; set; }

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

    public string? VoiceDefaultServerId { get; set; }

    public string? VoiceDefaultServerUrl { get; set; }

    public string? VoiceDefaultPath { get; set; }

    public string? VoiceDefaultType { get; set; }

    public string? VoiceDefaultData { get; set; }

    public string? VoiceThumServerUrl { get; set; }

    public string? VoiceThumPath { get; set; }

    public string? VoiceThumData { get; set; }

    public bool? VoiceIsLoading { get; set; }

    public bool? VoiceIsDownload { get; set; }

    public bool? VoiceIsSaved { get; set; }

    public bool? VoiceIsViewOnly { get; set; }

    public int? VoiceFromUserId { get; set; }

    public int? VoiceFromMobileNumberId { get; set; }

    public int? VoiceToUserId { get; set; }

    public int? VoiceToMobileNumberId { get; set; }

    public int? LocationRequestFromRemoteUserId { get; set; }

    public int? LocationRequestToRemoteUserId { get; set; }

    public string? LocationType { get; set; }

    public string? LocationLink { get; set; }

    public string? LocationLongitude { get; set; }

    public string? LocationLatitude { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? StateName { get; set; }

    public string? CityName { get; set; }

    public string? AddressLine { get; set; }

    public string? StaticMapUrl { get; set; }

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

    public bool? IsVisible { get; set; }

    public bool? IsDelivered { get; set; }

    public bool? IsActive { get; set; }

    public decimal? UploadDayUtc { get; set; }

    public int? UploadMonthUtc { get; set; }

    public int? UploadYearUtc { get; set; }

    public string? UploadTimeUtc { get; set; }

    public DateTime? UploadDateUtc { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public int? ScreenChatShowId7 { get; set; }

    public int? ScreenChatShowId6 { get; set; }

    public int? ScreenChatShowId5 { get; set; }

    public int? ScreenChatShowId4 { get; set; }

    public int? ScreenChatShowId3 { get; set; }

    public int? ScreenChatShowId2 { get; set; }

    public int? ScreenChatShowId1 { get; set; }
}
