using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationResourceSectionList
{
    public long CmssectionId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? CmssectionCode { get; set; }

    public string? CmssectionName { get; set; }

    public string? CmssectionTitle { get; set; }

    public string? CmssectionDescription { get; set; }

    public string? CmssectionType { get; set; }

    public string? CmssectionText { get; set; }

    public string? CmssectionValue { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
