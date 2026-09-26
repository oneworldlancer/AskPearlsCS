using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPostShareVideoTraceList
{
    public long TraceId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? VideoId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? VideoCode { get; set; }

    public string? VideoType { get; set; }

    public string? VideoName { get; set; }

    public string? VideoTitle { get; set; }

    public string? VideoMimeType { get; set; }

    public string? VideoDescription { get; set; }

    public byte[]? VideoData { get; set; }

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
