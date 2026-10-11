using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserWebsiteWidgetQuestionAnswerList
{
    public long UwebsiteWidgetAnswerId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? CompanyName { get; set; }

    public string? WidgetTokenId { get; set; }

    public string? WidgetQuestionTokenId { get; set; }

    public string? WidgetQuestionCode { get; set; }

    public string? WidgetQuestionType { get; set; }

    public string? WidgetQuestionKey { get; set; }

    public string? WidgetAnswerTokenId { get; set; }

    public string? WidgetAnswerCode { get; set; }

    public string? WidgetAnswerType { get; set; }

    public string? WidgetAnswerKey { get; set; }

    public string? WidgetAnswerTitle { get; set; }

    public string? WidgetAnswerName { get; set; }

    public string? WidgetAnswerText { get; set; }

    public string? WidgetAnswerDescription { get; set; }

    public int? WidgetAnswerSortOrder { get; set; }

    public string? WebsiteTokenId { get; set; }

    public string? WebsiteName { get; set; }

    public string? WidgetPublicKey { get; set; }

    public string? WidgetVersionTokenId { get; set; }

    public string? WidgetVersionNumber { get; set; }

    public string? WidgetConfigurationJson { get; set; }

    public string? WidgetStatus { get; set; }

    public string? WidgetPublishedVersion { get; set; }

    public string? WebsiteCountry { get; set; }

    public string? WebsiteCity { get; set; }

    public string? WebsiteStreetName { get; set; }

    public string? WebsiteAddressLine { get; set; }

    public string? WebsiteAddress { get; set; }

    public string? WebsiteZipCode { get; set; }

    public string? WebsiteType { get; set; }

    public string? WebsiteHeight { get; set; }

    public string? StoreysNumber { get; set; }

    public string? DwellingsNumber { get; set; }

    public string? WebsiteDay { get; set; }

    public string? WebsiteMonth { get; set; }

    public string? WebsiteYear { get; set; }

    public string? WebsiteDate { get; set; }

    public string? WebsiteUprn { get; set; }

    public string? EngcodeTokenId { get; set; }

    public string? Hrbnumber { get; set; }

    public string? ImageTokenId { get; set; }

    public string? ImageServerUrl { get; set; }

    public string? ImageGoogleTokenId { get; set; }

    public string? ImageGoogleName { get; set; }

    public string? ImageGoogleServerUrl { get; set; }

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
