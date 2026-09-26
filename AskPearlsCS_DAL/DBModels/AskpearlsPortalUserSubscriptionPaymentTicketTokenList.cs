using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserSubscriptionPaymentTicketTokenList
{
    public long UsubscriptionTicketTokenId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? SubscriptionCode { get; set; }

    public string? SubscriptionType { get; set; }

    public string? SubscriptionMethodBy { get; set; }

    public string? SubscriptionTicketTokenId { get; set; }

    public string? SubscriptionUrlpaymentTokenId { get; set; }

    public string? SubscriptionApicontextId { get; set; }

    public string? SubscriptionAgreementId { get; set; }

    public string? SubscriptionPlanId { get; set; }

    public string? SubscriptionInerval { get; set; }

    public string? SubscriptionPayerId { get; set; }

    public string? SubscriptionAmount { get; set; }

    public string? SubscriptionGateway { get; set; }

    public string? SubscriptionPayerFirstName { get; set; }

    public string? SubscriptionPayerLastName { get; set; }

    public string? SubscriptionPayerEmailAddress { get; set; }

    public string? SubscriptionMilliSecStartDateTime { get; set; }

    public string? SubscriptionMilliSecEndDateTime { get; set; }

    public string? SubscriptionMilliSecRemindDateTime { get; set; }

    public DateTime? SubscriptionStartDateTime { get; set; }

    public DateTime? SubscriptionEndDateTime { get; set; }

    public DateTime? SubscriptionRemindDateTime { get; set; }

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

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
