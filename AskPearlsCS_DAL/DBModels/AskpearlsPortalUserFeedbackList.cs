using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserFeedbackList
{
    public long FeedBackId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? FeedBackCode { get; set; }

    public string? FeedBackSubject { get; set; }

    public string? FeedBackEmail { get; set; }

    public string? FeedBackMessage { get; set; }

    public string? FeedBackReply { get; set; }

    public string? FeedBackTitle { get; set; }

    public string? FeedBackText { get; set; }

    public string? FeedBackValue { get; set; }

    public string? WhatLike { get; set; }

    public string? WhatNotLike { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
