using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPaymentPaybackList
{
    public long PaymentPaybackId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PaymentPaybackTokenId { get; set; }

    public string? PaymentPaybackCode { get; set; }

    public string? PaymentPaybackName { get; set; }

    public string? PaymentPaybackTitle { get; set; }

    public string? PaymentPaybackDescription { get; set; }

    public int? PaymentPaybackOrder { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
