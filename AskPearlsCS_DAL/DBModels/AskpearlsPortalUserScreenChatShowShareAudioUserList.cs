using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowShareAudioUserList
{
    public long ShareId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? AudioId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? AudioCode { get; set; }

    public string? AudioType { get; set; }

    public string? AudioName { get; set; }

    public string? AudioTitle { get; set; }

    public string? AudioMimeType { get; set; }

    public string? AudioDescription { get; set; }

    public byte[]? AudioData { get; set; }

    public string? AudioWebUrl { get; set; }

    public string? AudioSmallUrl { get; set; }

    public string? AudioSmallLiveUrl { get; set; }

    public string? AudioSmallAwayUrl { get; set; }

    public string? AudioMidUrl { get; set; }

    public string? AudioLargeUrl { get; set; }

    public string? AudioThmUrl1 { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsDownload { get; set; }

    public bool? IsView { get; set; }

    public bool? IsSave { get; set; }

    public bool? IsAvatar { get; set; }

    public bool? IsRecent { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsPersonal { get; set; }

    public string? Password { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
