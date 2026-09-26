using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserTokenSpendList
{
    public long UspendId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? TokenSpendCode { get; set; }

    public string? TipSpendCode { get; set; }

    public string? RoomId { get; set; }

    public string? RoomSessionId { get; set; }

    public string? StudioId { get; set; }

    public bool? IsModelStudio { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserRole { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? TokenAmount { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public DateTime? TokenTime { get; set; }

    public string? TokenPricePerOneText { get; set; }

    public decimal? TokenPricePerOneValue { get; set; }

    public string? TokenCostTotalText { get; set; }

    public decimal? TokenCostTotalValue { get; set; }

    public string? ModelTokenAmount { get; set; }

    public string? ModelTokenPricePerOneText { get; set; }

    public decimal? ModelTokenPricePerOneValue { get; set; }

    public string? ModelTokenCostTotalText { get; set; }

    public decimal? ModelTokenCostTotalValue { get; set; }

    public string? SiteTokenAmount { get; set; }

    public string? SiteTokenPricePerOneText { get; set; }

    public decimal? SiteTokenPricePerOneValue { get; set; }

    public string? SiteTokenCostTotalText { get; set; }

    public decimal? SiteTokenCostTotalValue { get; set; }

    public int? LoginDay { get; set; }

    public int? LoginMonth { get; set; }

    public int? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public int? LogoutDay { get; set; }

    public int? LogoutMonth { get; set; }

    public int? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsSaved { get; set; }

    public bool? IsTemp { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
