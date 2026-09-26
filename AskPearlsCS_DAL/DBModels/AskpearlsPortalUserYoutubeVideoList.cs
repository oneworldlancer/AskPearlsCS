using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserYoutubeVideoList
{
    public long UserVideoId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? YouTubeVideoId { get; set; }

    public string? YouTubeVideoUrl { get; set; }

    public string? YouTubeVideoIframe { get; set; }

    public string? YouTubeVideoType { get; set; }

    public string? YouTubeVideoFileName { get; set; }

    public string? YouTubeVideoFileTitle { get; set; }

    public string? YouTubeVideoTitle { get; set; }

    public string? YouTubeVideoDescription { get; set; }

    public string? YouTubeVideoCategory { get; set; }

    public string? YouTubeVideoCategoryCode { get; set; }

    public string? YouTubeVideoKeywords { get; set; }

    public string? YouTubeVideoThm0 { get; set; }

    public string? YouTubeVideoThm1 { get; set; }

    public string? YouTubeVideoThm2 { get; set; }

    public string? YouTubeVideoThm3 { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsUpload { get; set; }

    public bool? IsPending { get; set; }

    public string? FromUserId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? FromUserInviteRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
