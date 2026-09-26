using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowMessageList
{
    public long MsgScreenChatShowId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? MembershipType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? ProjectTokenId { get; set; }

    public string? FromApplicationRoleCode { get; set; }

    public string? ToApplicationRoleCode { get; set; }

    public string? FromProjectRoleCode { get; set; }

    public string? ToProjectRoleCode { get; set; }

    public string? MessageTokenId { get; set; }

    public string? Tag { get; set; }

    public string? InviteTag { get; set; }

    public bool? IsDating { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerMobileNumberId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenCallShowTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? GroupTokenId { get; set; }

    public string? PrivateGroupTokenId { get; set; }

    public string? CallTokenId { get; set; }

    public string? GroupType { get; set; }

    public string? FromUserTokenId { get; set; }

    public string? FromMobileNumberTokenId { get; set; }

    public string? ToUserTokenId { get; set; }

    public string? ToMobileNumberTokenId { get; set; }

    public string? RemoteUserTokenId { get; set; }

    public string? RemoteMobileNumberTokenId { get; set; }

    public string? VoiceShowInviteId { get; set; }

    public string? VideoShowInviteId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserName { get; set; }

    public string? VisibleStatus { get; set; }

    public string? InviteCode { get; set; }

    public string? InviteType { get; set; }

    public string? MessageCode { get; set; }

    public string? MessageType { get; set; }

    public string? MessageText { get; set; }

    public string? MessageTextCallIn { get; set; }

    public string? MessageTextCallOut { get; set; }

    public string? MessageTextOriginalCode { get; set; }

    public string? MessageTextOriginal { get; set; }

    public string? MessageTextTranslateCode { get; set; }

    public string? MessageTextTranslated { get; set; }

    public string? CallCode { get; set; }

    public string? CallType { get; set; }

    public string? CallDirection { get; set; }

    public string? CallStatus { get; set; }

    public string? CallStatusIn { get; set; }

    public string? CallStatusOut { get; set; }

    public string? CallState { get; set; }

    public string? CallDuration { get; set; }

    public string? DateTimeText { get; set; }

    public string? DateTime { get; set; }

    public string? DateText { get; set; }

    public string? ServerDateDay { get; set; }

    public string? ServerDateMonth { get; set; }

    public string? ServerDateYear { get; set; }

    public string? UserDateDay { get; set; }

    public string? UserDateMonth { get; set; }

    public string? UserDateYear { get; set; }

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

    public string? ImageFromUserId { get; set; }

    public string? ImageFromMobileNumberId { get; set; }

    public string? ImageToUserId { get; set; }

    public string? ImageToMobileNumberId { get; set; }

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

    public string? VideoFromUserId { get; set; }

    public string? VideoFromMobileNumberId { get; set; }

    public string? VideoToUserId { get; set; }

    public string? VideoToMobileNumberId { get; set; }

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

    public string? VoiceFromUserId { get; set; }

    public string? VoiceFromMobileNumberId { get; set; }

    public string? VoiceToUserId { get; set; }

    public string? VoiceToMobileNumberId { get; set; }

    public string? LocationRequestFromRemoteUserId { get; set; }

    public string? LocationRequestToRemoteUserId { get; set; }

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

    public string? SuggestContactListFromRemoteUserId { get; set; }

    public string? SuggestContactListToRemoteUserId { get; set; }

    public string? SuggestContactListUserId { get; set; }

    public string? SuggestContactListMobileNumberId { get; set; }

    public bool? IsShareLocationAskRemoteEnable { get; set; }

    public bool? IsSubjectLine { get; set; }

    public bool? IsHistoryLine { get; set; }

    public bool? IsSpaceLine { get; set; }

    public bool? IsSessionClosedLine { get; set; }

    public bool? IsCreateNewChatLine { get; set; }

    public bool? IsDelivered { get; set; }

    public decimal? UploadDayUtc { get; set; }

    public int? UploadMonthUtc { get; set; }

    public int? UploadYearUtc { get; set; }

    public string? UploadTimeUtc { get; set; }

    public DateTime? UploadDateUtc { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public string? ActiveStatus { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
