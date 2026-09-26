using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGalleryImageList
{
    public long UgalleryImageId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? GalleryId { get; set; }

    public string? ImageTokenId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageLength { get; set; }

    public string? ImageDescription { get; set; }

    public byte[]? ImageData { get; set; }

    public string? ImageSmallUrl { get; set; }

    public string? ImageSmallLiveUrl { get; set; }

    public string? ImageSmallAwayUrl { get; set; }

    public string? ImageMimeTypeSmall { get; set; }

    public byte[]? ImageDataSmall { get; set; }

    public string? ImageMidUrl { get; set; }

    public string? ImageMimeTypeMid { get; set; }

    public byte[]? ImageDataMid { get; set; }

    public string? ImageLargeUrl { get; set; }

    public string? ImageMimeTypeLarge { get; set; }

    public byte[]? ImageDataLarge { get; set; }

    public bool? IsProfile { get; set; }

    public bool? IsAvatar { get; set; }

    public bool? IsPersonalAvatar { get; set; }

    public bool? IsDatingAvatar { get; set; }

    public bool? IsRecent { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsPersonal { get; set; }

    public string? Password { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSystemUser { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
