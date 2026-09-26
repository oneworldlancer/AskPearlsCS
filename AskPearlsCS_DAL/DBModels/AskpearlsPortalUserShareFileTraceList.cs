using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserShareFileTraceList
{
    public long TraceId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? FileId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? FileCode { get; set; }

    public string? FileType { get; set; }

    public string? FileName { get; set; }

    public string? FileTitle { get; set; }

    public string? FileMimeType { get; set; }

    public string? FileDescription { get; set; }

    public byte[]? FileData { get; set; }

    public string? FileWebUrl { get; set; }

    public string? FileSmallUrl { get; set; }

    public string? FileSmallLiveUrl { get; set; }

    public string? FileSmallAwayUrl { get; set; }

    public string? FileMidUrl { get; set; }

    public string? FileLargeUrl { get; set; }

    public string? FileThmUrl1 { get; set; }

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
