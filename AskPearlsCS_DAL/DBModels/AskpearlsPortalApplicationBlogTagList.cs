using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationBlogTagList
{
    public long BlogTagId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? BlogTagTokenId { get; set; }

    public string? BlogTagCode { get; set; }

    public string? BlogTagName { get; set; }

    public string? BlogTagTitle { get; set; }

    public string? BlogTagDescription { get; set; }

    public string? BlogTagType { get; set; }

    public string? BlogTagText { get; set; }

    public string? BlogTagValue { get; set; }

    public bool? BlogTagIsSystem { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
