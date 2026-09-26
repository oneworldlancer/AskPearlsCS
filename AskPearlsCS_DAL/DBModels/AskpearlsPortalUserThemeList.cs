using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserThemeList
{
    public long UthemeId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public int? UserId { get; set; }

    public int? MobileNumberId { get; set; }

    public int? ThemeCategoryId { get; set; }

    public int? ThemeSetId { get; set; }

    public string? ThemeTokenId { get; set; }

    public string? ThemeCode { get; set; }

    public string? ThemeValue { get; set; }

    public string? ThemeName { get; set; }

    public string? ThemeTitle { get; set; }

    public string? ThemeDescription { get; set; }

    public string? ThemeImageSmallUrl { get; set; }

    public string? ThemeImageMidUrl { get; set; }

    public string? ThemeImageLargeUrl { get; set; }

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
