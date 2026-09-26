using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserForumMyPinMessageList
{
    public long UmyPinForumId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? ForumTokenId { get; set; }

    public string? UforumTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ForumCategoryCode { get; set; }

    public string? ForumCategoryTitle { get; set; }

    public string? ForumMessageCode { get; set; }

    public string? ForumMessageSubject { get; set; }

    public string? ForumMessageEmail { get; set; }

    public string? ForumMessageText { get; set; }

    public string? ForumMessageReply { get; set; }

    public string? ForumMessageTitle { get; set; }

    public string? ForumMessageValue { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsAnswer { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
