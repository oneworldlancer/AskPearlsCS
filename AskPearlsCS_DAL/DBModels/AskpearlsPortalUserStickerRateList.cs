using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserStickerRateList
{
    public long UrateId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? UstickerId { get; set; }

    public string? RateValue { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? StickerCategoryId { get; set; }

    public string? StickerSetId { get; set; }

    public string? StickerSetTokenId { get; set; }

    public string? StickerCode { get; set; }

    public string? StickerValue { get; set; }

    public string? StickerCostAmount { get; set; }

    public string? StickerName { get; set; }

    public string? StickerTitle { get; set; }

    public string? StickerDescription { get; set; }

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
