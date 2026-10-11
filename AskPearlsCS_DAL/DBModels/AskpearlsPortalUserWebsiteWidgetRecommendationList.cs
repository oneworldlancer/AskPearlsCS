using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserWebsiteWidgetRecommendationList
{
    public long UwebsiteWidgetRecommendationId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? CompanyName { get; set; }

    public string? WebsiteTokenId { get; set; }

    public string? WidgetTokenId { get; set; }

    public string? WidgetPublicKey { get; set; }

    public string? WidgetVersionTokenId { get; set; }

    public string? WidgetVersionNumber { get; set; }

    public string? RecommendationTokenId { get; set; }

    public string? RecommendationKey { get; set; }

    public string? RecommendationCode { get; set; }

    public string? RecommendationType { get; set; }

    public string? RecommendationName { get; set; }

    public string? RecommendationTitle { get; set; }

    public string? RecommendationText { get; set; }

    public string? RecommendationShortDescription { get; set; }

    public string? RecommendationDescription { get; set; }

    public string? RecommendationImageTokenId { get; set; }

    public string? RecommendationImageUrl { get; set; }

    public string? RecommendationUrl { get; set; }

    public string? RecommendationCta { get; set; }

    public string? RecommendationCtacode { get; set; }

    public string? RecommendationCtatype { get; set; }

    public string? RecommendationCtatext { get; set; }

    public string? RecommendationCtaimageTokenId { get; set; }

    public string? RecommendationCtaimageUrl { get; set; }

    public string? RecommendationCtaurl { get; set; }

    public string? RecommendationHighlightsJson { get; set; }

    public string? ImageTokenId { get; set; }

    public string? ImageServerUrl { get; set; }

    public string? ImageGoogleTokenId { get; set; }

    public string? ImageGoogleName { get; set; }

    public string? ImageGoogleServerUrl { get; set; }

    public int? RecommendationSortOrder { get; set; }

    public bool? IsNew { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVisible { get; set; }

    public DateTime? CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
