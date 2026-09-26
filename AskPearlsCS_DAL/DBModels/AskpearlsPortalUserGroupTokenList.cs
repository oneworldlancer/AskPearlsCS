using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGroupTokenList
{
    public long GroupId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public bool? IsDating { get; set; }

    public string? GroupTokenId { get; set; }

    public string? GroupCode { get; set; }

    public string? GroupKey { get; set; }

    public string? GroupCodeCount { get; set; }

    public string? GroupCount { get; set; }

    public string? GroupType { get; set; }

    public string? CategoryId { get; set; }

    public string? InviteOwnerUserId { get; set; }

    public string? InviteOwnerMobileNumberId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? UserId1 { get; set; }

    public string? MobileNumberId1 { get; set; }

    public string? UserId2 { get; set; }

    public string? MobileNumberId2 { get; set; }

    public string? UserId3 { get; set; }

    public string? MobileNumberId3 { get; set; }

    public string? UserId4 { get; set; }

    public string? MobileNumberId4 { get; set; }

    public string? UserId5 { get; set; }

    public string? MobileNumberId5 { get; set; }

    public string? AvatarName { get; set; }

    public string? GroupName { get; set; }

    public string? GroupTitle { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public byte[]? ImageData { get; set; }

    public string? UserType { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
