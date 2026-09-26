using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPostShareVideoYoutube
{
    public long ShareYouTubeVideoId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? UserSessionId { get; set; }

    public string? YouTubeVideoId { get; set; }

    public string? YouTubeChannelId { get; set; }

    public string? YouTubeVideoCode { get; set; }

    public string? YouTubeVideoType { get; set; }

    public string? YouTubeVideoName { get; set; }

    public string? YouTubeVideoTitle { get; set; }

    public string? VideoMimeType { get; set; }

    public string? VideoDescription { get; set; }

    public byte[]? VideoData { get; set; }

    public string? VideoThumImageMimeType { get; set; }

    public byte[]? VideoThumImageData { get; set; }

    public string? VideoWebUrl { get; set; }

    public string? VideoSmallUrl { get; set; }

    public string? VideoSmallLiveUrl { get; set; }

    public string? VideoSmallAwayUrl { get; set; }

    public string? VideoMidUrl { get; set; }

    public string? VideoLargeUrl { get; set; }

    public string? VideoThmUrl1 { get; set; }

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
