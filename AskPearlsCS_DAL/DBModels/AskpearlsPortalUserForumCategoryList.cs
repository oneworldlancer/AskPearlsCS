using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserForumCategoryList
{
    public long ForumCategoryId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? ForumCategoryTokenId { get; set; }

    public bool? IsPersonalAccount { get; set; }

    public bool? IsDatingAccount { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ForumCategoryCode { get; set; }

    public string? ForumCategoryName { get; set; }

    public string? ForumCategoryTitle { get; set; }

    public string? ForumCategoryDescription { get; set; }

    public string? ForumCategoryColor { get; set; }

    public int? ForumCategoryOrder { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public byte[]? ImageData { get; set; }

    public string? UserType { get; set; }

    public DateTime? DateAdd { get; set; }

    public DateTime? DateRemove { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
