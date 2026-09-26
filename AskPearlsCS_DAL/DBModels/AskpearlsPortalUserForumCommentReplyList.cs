using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserForumCommentReplyList
{
    public long ReplyId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? ForumId { get; set; }

    public string? ForumTokenId { get; set; }

    public string? CommentId { get; set; }

    public string? CommentTokenId { get; set; }

    public string? ReplyTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? ReplyTextCss { get; set; }

    public string? ReplyCode { get; set; }

    public string? ReplyType { get; set; }

    public string? ReplyText { get; set; }

    public string? FileId { get; set; }

    public string? FileUrl { get; set; }

    public string? FileDuration { get; set; }

    public int? FriendId { get; set; }

    public int? FriendMobileNumberId { get; set; }

    public int? RemoteUserId { get; set; }

    public int? RemoteMobileNumberId { get; set; }

    public int? OwnerUserId { get; set; }

    public int? OwnerMobileNumberId { get; set; }

    public string? UserType { get; set; }

    public DateTime? DateAdd { get; set; }

    public DateTime? DateRemove { get; set; }

    public string? BlackListStatus { get; set; }

    public bool? IsBlackList { get; set; }

    public string? MatchLikeStatus { get; set; }

    public bool? IsMatch { get; set; }

    public bool? IsDatingMatchConnect { get; set; }

    public bool? IsLike { get; set; }

    public bool? IsDating { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsAnswer { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public int? RemoveDay { get; set; }

    public int? RemoveMonth { get; set; }

    public int? RemoveYear { get; set; }

    public DateTime? RemoveDate { get; set; }

    public string? UploadTime { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
