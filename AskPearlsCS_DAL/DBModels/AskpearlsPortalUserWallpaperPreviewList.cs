using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserWallpaperPreviewList
{
    public long UpreviewId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? WallpaperId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? CategoryId { get; set; }

    public string? WallpaperSetId { get; set; }

    public string? WallpaperSetTokenId { get; set; }

    public string? ViewCount { get; set; }

    public string? WallpaperCode { get; set; }

    public string? WallpaperValue { get; set; }

    public string? WallpaperCostAmount { get; set; }

    public string? WallpaperName { get; set; }

    public string? WallpaperTitle { get; set; }

    public string? WallpaperDescription { get; set; }

    public string? WallpaperImageUrl { get; set; }

    public string? WallpaperImageSmallUrl { get; set; }

    public string? WallpaperImageMidUrl { get; set; }

    public string? WallpaperImageLargeUrl { get; set; }

    public string? ImageTokenId { get; set; }

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

    public string? ImageMimeTypeSmall { get; set; }

    public byte[]? ImageDataSmall { get; set; }

    public string? ImageMidUrl { get; set; }

    public string? ImageMimeTypeMid { get; set; }

    public byte[]? ImageDataMid { get; set; }

    public string? ImageLargeUrl { get; set; }

    public string? ImageMimeTypeLarge { get; set; }

    public byte[]? ImageDataLarge { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsTlknEmotion { get; set; }

    public bool? IsUser { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsView { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
