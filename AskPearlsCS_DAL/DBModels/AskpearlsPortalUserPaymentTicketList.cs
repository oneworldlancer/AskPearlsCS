using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserPaymentTicketList
{
    public long PaymentTicketId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? PaymentTokenId { get; set; }

    public int? PaymentTicketDay { get; set; }

    public int? PaymentTicketMonth { get; set; }

    public int? PaymentTicketYear { get; set; }

    public string? PaymentMonthTokenId { get; set; }

    public string? PaymentYearTokenId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public bool? IsNextPaymentTicket { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
