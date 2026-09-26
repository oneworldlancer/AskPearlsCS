using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPaymentPaybackHistoryList
{
    public long UpaymentPaybackHistoryId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PaymentPaybackTokenId { get; set; }

    public string? PaymentPaybackCode { get; set; }

    public string? UserPaymentPaybackTokenId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? PaymentPaybackAccountId { get; set; }

    public string? PaymentPaybackFirstName { get; set; }

    public string? PaymentPaybackFirstNameLower { get; set; }

    public string? PaymentPaybackLastName { get; set; }

    public string? PaymentPaybackLastNameLower { get; set; }

    public string? PaymentPaybackFullName { get; set; }

    public string? PaymentPaybackFullNameLower { get; set; }

    public string? PaymentPaybackEmailAddress { get; set; }

    public string? PaymentPaybackEmailAddressLower { get; set; }

    public string? PaymentPaybackPassword { get; set; }

    public bool? IsValid { get; set; }

    public bool? IsVerify { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
