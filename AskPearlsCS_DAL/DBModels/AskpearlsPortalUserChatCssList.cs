using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserChatCssList
{
    public long StyleId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? LoungeId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? StyleBackgroundColor { get; set; }

    public string? StyleContainerBackgroundColor { get; set; }

    public string? StyleContainerBorderColor { get; set; }

    public string? StyleHeadingTextColor { get; set; }

    public string? StyleHeadingLinkColor { get; set; }

    public string? StyleDefaultTextColor { get; set; }

    public string? StyleDefaultLinkColor { get; set; }

    public string? CssBackgroundColor { get; set; }

    public string? CssBackgroundImageUrl { get; set; }

    public bool? IsCssBackgroundColor { get; set; }

    public bool? IsCssBackgroundImageUrl { get; set; }

    public string? CssBackgroundPosition { get; set; }

    public string? CssBackgroundRepeat { get; set; }

    public bool? IsCssBackgroundMro { get; set; }

    public string? CssBackgroundMrourl { get; set; }

    public string? CssContainerBackgroundColor { get; set; }

    public string? CssContainerBorderColor { get; set; }

    public string? CssContainerBorderStyle { get; set; }

    public string? CssContainerBorderSize { get; set; }

    public string? CssContainerTransparence { get; set; }

    public string? CssHeaderBackgroundColor { get; set; }

    public string? CssHeaderTextColor { get; set; }

    public string? CssHeaderTextStye { get; set; }

    public string? CssHeaderTextSize { get; set; }

    public bool? IsCssHeaderTextBold { get; set; }

    public bool? IsCssHeaderTextItalic { get; set; }

    public string? CssHeaderLinkColor { get; set; }

    public string? CssHeaderLinkStye { get; set; }

    public string? CssHeaderLinkSize { get; set; }

    public bool? IsCssHeaderLinkBold { get; set; }

    public bool? IsCssHeaderLinkItalic { get; set; }

    public string? CssDefaultBackgroundColor { get; set; }

    public string? CssDefaultTextColor { get; set; }

    public string? CssDefaultTextStye { get; set; }

    public string? CssDefaultTextSize { get; set; }

    public bool? IsCssDefaultTextBold { get; set; }

    public bool? IsCssDefaultTextItalic { get; set; }

    public string? CssDefaultLinkColor { get; set; }

    public string? CssDefaultLinkStye { get; set; }

    public string? CssDefaultLinkSize { get; set; }

    public bool? IsCssDefaultLinkBold { get; set; }

    public bool? IsCssDefaultLinkItalic { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
