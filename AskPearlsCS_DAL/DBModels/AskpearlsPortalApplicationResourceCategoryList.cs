using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationResourceCategoryList
{
    public long CmscategoryId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? CmscategoryCode { get; set; }

    public string? CmscategoryName { get; set; }

    public string? CmscategoryTitle { get; set; }

    public string? CmscategoryDescription { get; set; }

    public string? CmscategoryType { get; set; }

    public string? CmscategoryText { get; set; }

    public string? CmscategoryValue { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
