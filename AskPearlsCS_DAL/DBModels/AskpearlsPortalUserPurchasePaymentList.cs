using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPurchasePaymentList
{
    public long UpurchaseId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? PurchaseCode { get; set; }

    public string? PurchaseType { get; set; }

    public string? PurchaseMethodBy { get; set; }

    public string? PurchaseTicketTokenId { get; set; }

    public string? PurchaseUrlpaymentTokenId { get; set; }

    public string? PurchaseApicontextId { get; set; }

    public string? PurchasePayerId { get; set; }

    public string? PurchasePayerFirstName { get; set; }

    public string? PurchasePayerLastName { get; set; }

    public string? PurchasePayerEmailAddress { get; set; }

    public string? PurchasePackageTokenId { get; set; }

    public string? PurchasePackageCost { get; set; }

    public string? PurchasePaymentId { get; set; }

    public string? PurchaseAgreementId { get; set; }

    public string? PurchasePlanId { get; set; }

    public string? PurchaseInerval { get; set; }

    public string? PurchaseMilliSecStartDateTime { get; set; }

    public string? PurchaseMilliSecEndDateTime { get; set; }

    public string? PurchaseMilliSecRemindDateTime { get; set; }

    public DateTime? PurchaseStartDateTime { get; set; }

    public DateTime? PurchaseEndDateTime { get; set; }

    public DateTime? PurchaseRemindDateTime { get; set; }

    public string? PurchaseTokenType { get; set; }

    public string? PurchasePaymentProcessorText { get; set; }

    public string? PurchasePaymentProcessorValue { get; set; }

    public string? PurchasePaymentProcessorCode { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? TokenAmount { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public string? TokenPricePerOneText { get; set; }

    public decimal? TokenPricePerOneValue { get; set; }

    public string? TokenCostTotalText { get; set; }

    public decimal? TokenCostTotalValue { get; set; }

    public int? SiteTokenAmount { get; set; }

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

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
