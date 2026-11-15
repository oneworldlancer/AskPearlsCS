namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserNotificationList
{
    public long UNotificationId { get; set; }
    public Guid RowViewTokenId { get; set; }
    public string? RowDataTokenId { get; set; }
    public string? OwnerUserTokenId { get; set; }
    public string? OrganizationTokenId { get; set; }
    public string? CompanyTokenId { get; set; }
    public string? NotificationTokenId { get; set; }
    public string? NotificationCode { get; set; }
    public string? NotificationValue { get; set; }
    public string? NotificationType { get; set; }
    public string? NotificationText { get; set; }
    public string? NotificationTitle { get; set; }
    public string? NotificationMessage { get; set; }
    public string? ActionUrl { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public bool? IsVisible { get; set; }
    public bool? IsActive { get; set; }
    public int? UploadDay { get; set; }
    public int? UploadMonth { get; set; }
    public int? UploadYear { get; set; }
    public DateTime? UploadDate { get; set; }
}
