using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserStickerCountList
{
    public long UstickerCountId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? StickerSetId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? StickerSetTokenId { get; set; }

    public string? StickerSetCount { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryMiniDescription { get; set; }

    public string? CategoryFullDescription { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryTitle { get; set; }

    public string? CategoryDescription { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCount { get; set; }

    public string? StickerImageSmallUrl { get; set; }

    public string? StickerImageMidUrl { get; set; }

    public string? StickerImageLargeUrl { get; set; }

    public bool? IsCostFree { get; set; }

    public decimal? CostPrice { get; set; }

    public int? CategoryOrder { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsPersonal { get; set; }

    public string? Password { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsAvatar { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsView { get; set; }

    public bool? IsRecent { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
