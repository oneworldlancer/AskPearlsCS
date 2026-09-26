using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSettingList
{
    public long SettingId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? SettingCode { get; set; }

    public string? SettingText { get; set; }

    public string? SettingValue { get; set; }

    public string? SettingLocalhost { get; set; }

    public string? SettingWebServer { get; set; }

    public string? SettingType { get; set; }

    public string? SettingCategory { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
