using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserStickerSetList
{
    public long UstickerSetId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public int? UserId { get; set; }

    public int? MobileNumberId { get; set; }

    public int? CategoryId { get; set; }

    public string? StickerSetTokenId { get; set; }

    public string? StickerSetTitle { get; set; }

    public string? StickerSetDescription { get; set; }

    public string? StickerCategoryId { get; set; }

    public string? CategoryCode { get; set; }

    public string? CategoryMiniDescription { get; set; }

    public string? CategoryFullDescription { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryTitle { get; set; }

    public string? CategoryDescription { get; set; }

    public string? ImageTitle { get; set; }

    public int? ImageCount { get; set; }

    public string? StickerImageSmallUrl { get; set; }

    public string? StickerImageMidUrl { get; set; }

    public string? StickerImageLargeUrl { get; set; }

    public bool? IsCostFree { get; set; }

    public decimal? CostPrice { get; set; }

    public int? CategoryOrder { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsAskpearlscontent { get; set; }

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
