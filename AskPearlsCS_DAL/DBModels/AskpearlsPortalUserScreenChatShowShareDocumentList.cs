using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserScreenChatShowShareDocumentList
{
    public long DocumentId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? DocumentTokenId { get; set; }

    public string? ScreenChatShowId { get; set; }

    public string? ScreenChatShowTicketId { get; set; }

    public string? ScreenOwnerUserId { get; set; }

    public string? GroupId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? UserSessionId { get; set; }

    public string? DocumentCode { get; set; }

    public string? DocumentType { get; set; }

    public string? DocumentName { get; set; }

    public string? DocumentTitle { get; set; }

    public string? DocumentMimeType { get; set; }

    public string? DocumentDescription { get; set; }

    public string? DocumentDuration { get; set; }

    public string? DocumentSize { get; set; }

    public string? DocumentExtension { get; set; }

    public byte[]? DocumentData { get; set; }

    public string? DocumentThumImageMimeType { get; set; }

    public byte[]? DocumentThumImageData { get; set; }

    public string? DocumentWebUrl { get; set; }

    public string? DocumentSmallUrl { get; set; }

    public string? DocumentSmallLiveUrl { get; set; }

    public string? DocumentSmallAwayUrl { get; set; }

    public string? DocumentoMidUrl { get; set; }

    public string? DocumentLargeUrl { get; set; }

    public string? DocumentThmUrl { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsDownload { get; set; }

    public bool? IsView { get; set; }

    public bool? IsSave { get; set; }

    public bool? IsAvatar { get; set; }

    public bool? IsRecent { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsPersonal { get; set; }

    public string? Password { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
