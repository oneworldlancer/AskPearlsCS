using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationResourceList
{
    public long Cmsid { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public int? CmscategoryId { get; set; }

    public string? CmssectionCode { get; set; }

    public string? CmscategoryCode { get; set; }

    public string? Cmscategory { get; set; }

    public string? Cmscode { get; set; }

    public string? Cmstitle { get; set; }

    public string? Cmsvalue { get; set; }

    public string? Cmsname { get; set; }

    public string? Cmstype { get; set; }

    public string? Cmstext { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
