using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserBlogPostEventList
{
    public long UblogPostEventId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PostId { get; set; }

    public string? UpostTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? FriendId { get; set; }

    public string? FriendMobileNumberId { get; set; }

    public string? OwnerUserId { get; set; }

    public string? OwnerMobileNumberId { get; set; }

    public string? RemoteUserId { get; set; }

    public string? RemoteMobileNumberId { get; set; }

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

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public int? RemoveDay { get; set; }

    public int? RemoveMonth { get; set; }

    public int? RemoveYear { get; set; }

    public DateTime? RemoveDate { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
