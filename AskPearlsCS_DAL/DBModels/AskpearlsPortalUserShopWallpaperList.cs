using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserShopWallpaperList
{
    public long UshopWallpaperId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentTokenId { get; set; }

    public string? PaymentEarnTokenId { get; set; }

    public string? UserType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? WallpaperSetId { get; set; }

    public string? WallpaperSetTokenId { get; set; }

    public string? WallpaperId { get; set; }

    public string? WallpaperCategoryId { get; set; }

    public string? WallpaperCode { get; set; }

    public string? WallpaperValue { get; set; }

    public string? WallpaperName { get; set; }

    public string? WallpaperTitle { get; set; }

    public string? WallpaperDescription { get; set; }

    public string? WallpaperImageSmallUrl { get; set; }

    public string? WallpaperImageMidUrl { get; set; }

    public string? WallpaperImageLargeUrl { get; set; }

    public bool? IsCostFree { get; set; }

    public decimal? CostPrice { get; set; }

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
