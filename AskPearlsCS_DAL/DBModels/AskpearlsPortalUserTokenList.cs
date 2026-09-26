using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserTokenList
{
    public long TokenId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? PurchasePackageTokenId { get; set; }

    public string? TokenCode { get; set; }

    public string? TokenType { get; set; }

    public string? TokenCategory { get; set; }

    public string? TokenTitle { get; set; }

    public string? TokenText { get; set; }

    public int? ItemOrder { get; set; }

    public bool? IsSelected { get; set; }

    public string? TokenAmount { get; set; }

    public string? TokenAmountFree { get; set; }

    public string? TokenPricePerOneText { get; set; }

    public decimal? TokenPricePerOneValue { get; set; }

    public string? TokenCostTotalText { get; set; }

    public decimal? TokenCostTotalValue { get; set; }

    public string? PurchasePaymentProcessorText { get; set; }

    public string? PurchasePaymentProcessorValue { get; set; }

    public string? PurchasePaymentProcessorCode { get; set; }

    public string? TokenCurrencyText { get; set; }

    public string? TokenCurrencyValue { get; set; }

    public string? TokenCurrencyCode { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
