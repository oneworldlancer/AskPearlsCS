using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserForumQueryList
{
    public long ForumQueryMessageId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ContactUsCode { get; set; }

    public string? ContactUsSubject { get; set; }

    public string? ContactUsEmail { get; set; }

    public string? ContactUsMessage { get; set; }

    public string? ContactUsReply { get; set; }

    public string? ContactUsTitle { get; set; }

    public string? ContactUsText { get; set; }

    public string? ContactUsValue { get; set; }

    public bool? IsReply { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
