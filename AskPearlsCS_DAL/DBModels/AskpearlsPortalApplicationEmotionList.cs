using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationEmotionList
{
    public long ImageId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string RowDataTokenId { get; set; } = null!;

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageValue { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageDescription { get; set; }

    public string? ImageSmallUrl { get; set; }

    public string? ImageMidUrl { get; set; }

    public string? ImageLargeUrl { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsTlknEmotion { get; set; }

    public bool IsUser { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
