using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsUserTokenPurchase
{
    public long PurchaseId { get; set; }

    public Guid UpurchaseId { get; set; }

    public string? PurchaseOrderId { get; set; }

    public string? PurchaseTokenType { get; set; }

    public string? PurchasePaymentProcessorText { get; set; }

    public string? PurchasePaymentProcessorValue { get; set; }

    public string? PurchasePaymentProcessorCode { get; set; }

    public int? UserId { get; set; }

    public int? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public int? TokenAmount { get; set; }

    public int? PaymentTicketId { get; set; }

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

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
