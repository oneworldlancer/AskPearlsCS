using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserRssFeedList
{
    public long UrssFeedId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public int? UserId { get; set; }

    public int? MobileNumberId { get; set; }

    public int? RssFeedCategoryId { get; set; }

    public string? RssFeedCategoryTokenId { get; set; }

    public int? RssFeedGroupId { get; set; }

    public string? RssFeedSource { get; set; }

    public string? RssFeedType { get; set; }

    public string? RssFeedKey { get; set; }

    public string? RssFeedName { get; set; }

    public string? RssFeedTitle { get; set; }

    public int? RssFeedOrder { get; set; }

    public string? RssFeedDescription { get; set; }

    public string? RssFeedUrl { get; set; }

    public bool? IsCustomTitle { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public int? GroupOrder { get; set; }

    public byte[]? ImageData { get; set; }

    public string? UserType { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
