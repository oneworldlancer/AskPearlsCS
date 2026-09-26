using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationBlogCategoryList
{
    public long BlogCategoryId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? BlogCategoryTokenId { get; set; }

    public string? BlogCategoryCode { get; set; }

    public string? BlogCategoryName { get; set; }

    public string? BlogCategoryTitle { get; set; }

    public string? BlogCategoryDescription { get; set; }

    public string? BlogCategoryType { get; set; }

    public string? BlogCategoryText { get; set; }

    public string? BlogCategoryValue { get; set; }

    public bool? BlogCategoryIsSystem { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
