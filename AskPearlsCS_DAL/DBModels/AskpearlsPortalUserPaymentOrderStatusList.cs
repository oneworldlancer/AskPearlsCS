using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPaymentOrderStatusList
{
    public long UpaymentOrderId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentTokenId { get; set; }

    public string? PaymentEarnTokenId { get; set; }

    public string? PaymentMonthTokenId { get; set; }

    public string? PaymentYearTokenId { get; set; }

    public string? PayoneerIdmodel { get; set; }

    public bool? IsPayoneerVerifyModel { get; set; }

    public string? PayoneerIdstudio { get; set; }

    public bool? IsPayoneerVerifyStudio { get; set; }

    public string? ModelActiveStatus { get; set; }

    public string? StudioActiveStatus { get; set; }

    public int? PurchaseOrderId { get; set; }

    public string? PurchaseTokenType { get; set; }

    public string? PurchasePaymentProcessorText { get; set; }

    public string? PurchasePaymentProcessorValue { get; set; }

    public string? PurchasePaymentProcessorCode { get; set; }

    public int? StudioId { get; set; }

    public bool? IsModelStudio { get; set; }

    public int? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int? TipTokenAmount { get; set; }

    public int? NormalRoomTokenAmount { get; set; }

    public int? GroupShowTokenAmount { get; set; }

    public int? PrivateShowTokenAmount { get; set; }

    public int? SpyShowTokenAmount { get; set; }

    public int? PartyShowMemberTokenAmount { get; set; }

    public int? PartyShowModelTokenAmount { get; set; }

    public int? PartyShowTokenAmount { get; set; }

    public int? PmmessageTokenAmount { get; set; }

    public int? SalesTokenAmount { get; set; }

    public int? ReferralTokenAmount { get; set; }

    public int? TokenAmountTotal { get; set; }

    public string? TokenPricePerOneText { get; set; }

    public string? TokenCostTotalText { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public bool? IsNextPaymentTicket { get; set; }

    public int? PaymentTicketDay { get; set; }

    public int? PaymentTicketMonth { get; set; }

    public int? PaymentTicketYear { get; set; }

    public decimal? TokenPricePerOneValue { get; set; }

    public decimal? TokenCostTotalValue { get; set; }

    public int? LoginDay { get; set; }

    public int? LoginMonth { get; set; }

    public int? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public int? LogoutDay { get; set; }

    public int? LogoutMonth { get; set; }

    public int? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
