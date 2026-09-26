using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserTokenPurchaseList
{
    public long UpurchaseId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? PurchasePackageTokenId { get; set; }

    public string? PurchasePackageTicketId { get; set; }

    public string? PurchaseOrderId { get; set; }

    public string? PurchaseTokenType { get; set; }

    public string? PurchasePaymentProcessorText { get; set; }

    public string? PurchasePaymentProcessorValue { get; set; }

    public string? PurchasePaymentProcessorCode { get; set; }

    public string? TokenCode { get; set; }

    public string? TokenTag { get; set; }

    public string? TokenTagId { get; set; }

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

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
