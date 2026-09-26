using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserStickerImageList
{
    public long UstickerId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public int? UserId { get; set; }

    public int? MobileNumberId { get; set; }

    public int? CategoryId { get; set; }

    public int? StickerCategoryId { get; set; }

    public int? StickerSetId { get; set; }

    public string? StickerTokenId { get; set; }

    public string? StickerCode { get; set; }

    public string? StickerValue { get; set; }

    public string? StickerCostAmount { get; set; }

    public string? StickerName { get; set; }

    public string? StickerTitle { get; set; }

    public string? StickerDescription { get; set; }

    public string? StickerImageName { get; set; }

    public string? StickerImageUrl { get; set; }

    public string? StickerImageSmallUrl { get; set; }

    public string? StickerImageMidUrl { get; set; }

    public string? StickerImageLargeUrl { get; set; }

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

    public bool? IsAnimate { get; set; }

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
