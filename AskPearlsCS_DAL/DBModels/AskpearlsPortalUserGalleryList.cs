using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserGalleryList
{
    public long GalleryId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? GalleryTitle { get; set; }

    public string? GalleryMiniDescription { get; set; }

    public string? GalleryFullDescription { get; set; }

    public string? ImageName { get; set; }

    public string? ImageTitle { get; set; }

    public string? ImageDescription { get; set; }

    public string? ImageSmallUrl { get; set; }

    public string? ImageMidUrl { get; set; }

    public string? ImageLargeUrl { get; set; }

    public bool? IsGallery { get; set; }

    public bool? IsSystem { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsPersonal { get; set; }

    public string? Password { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsAvatar { get; set; }

    public bool? IsRecent { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
