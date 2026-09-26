using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationGroupRoleList
{
    public long UgroupRoleId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? CategoryId { get; set; }

    public string? CategoryTokenId { get; set; }

    public string? GroupTokenId { get; set; }

    public string? GroupRoleTokenId { get; set; }

    public string? GroupRoleCode { get; set; }

    public string? GroupRoleType { get; set; }

    public string? GroupRoleTitle { get; set; }

    public string? GroupRoleName { get; set; }

    public string? GroupCode { get; set; }

    public string? GroupType { get; set; }

    public string? GroupKey { get; set; }

    public string? AvatarName { get; set; }

    public string? GroupName { get; set; }

    public string? GroupTitle { get; set; }

    public string? GroupDescription { get; set; }

    public bool? IsCustomTitle { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageCode { get; set; }

    public string? ImageType { get; set; }

    public string? ImageMimeType { get; set; }

    public string? ImageDescription { get; set; }

    public string? GroupOrder { get; set; }

    public byte[]? ImageData { get; set; }

    public string? UserType { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
