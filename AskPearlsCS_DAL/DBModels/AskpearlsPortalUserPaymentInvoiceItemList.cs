using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPaymentInvoiceItemList
{
    public long UpaymentId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? UinvoiceOrderId { get; set; }

    public string? InvoiceOrderTokenId { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentTokenId { get; set; }

    public string? PaymentEarnTokenId { get; set; }

    public string? PaymentMonthTokenId { get; set; }

    public string? PaymentYearTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? PaymentTag { get; set; }

    public string? PaymentCode { get; set; }

    public string? PaymentCategory { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public int? TotalToken { get; set; }

    public decimal? TotalCredit { get; set; }

    public decimal? TotalCreditOriginal { get; set; }

    public string? ModelActiveStatus { get; set; }

    public int? TokenAmount { get; set; }

    public string? TokenCredit { get; set; }

    public string? TokenPricePerOneText { get; set; }

    public string? TokenCostTotalText { get; set; }

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
