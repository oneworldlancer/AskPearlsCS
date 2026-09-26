using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSoundList
{
    public long UsoundId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? SoundId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? SoundCode { get; set; }

    public string? SoundText { get; set; }

    public string? SoundValue { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
