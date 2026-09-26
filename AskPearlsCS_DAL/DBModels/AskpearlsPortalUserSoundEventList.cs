using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSoundEventList
{
    public long EventSoundId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? UserId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? EventSoundCode { get; set; }

    public string? EventSoundType { get; set; }

    public string? EventSoundShowId { get; set; }

    public string? EventSoundShowType { get; set; }

    public string? EventSoundText { get; set; }

    public string? EventSoundValue { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
