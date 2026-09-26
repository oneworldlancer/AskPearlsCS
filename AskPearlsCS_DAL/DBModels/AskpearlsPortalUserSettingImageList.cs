using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSettingImageList
{
    public long ImageId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public byte[]? ImageData { get; set; }

    public string? ImageSmallUrl { get; set; }

    public string? ImageSmallLiveUrl { get; set; }

    public string? ImageSmallAwayUrl { get; set; }

    public string? ImageMidUrl { get; set; }

    public string? ImageLargeUrl { get; set; }

    public string? ImageThmUrl1 { get; set; }

    public bool? IsProfile { get; set; }

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
