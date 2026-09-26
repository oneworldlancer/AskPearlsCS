using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserTokenTransferList
{
    public long UtransferId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? InviteTag { get; set; }

    public string? TransferCode { get; set; }

    public string? TransferTokenId { get; set; }

    public string? GroupId { get; set; }

    public string? UserId { get; set; }

    public string? MobileNumberId { get; set; }

    public string? UserSessionId { get; set; }

    public string? UserType { get; set; }

    public string? UserRole { get; set; }

    public string? FromUserId { get; set; }

    public string? FromMobileNumberId { get; set; }

    public string? FromUserSessionId { get; set; }

    public string? FromUserType { get; set; }

    public string? FromUserRole { get; set; }

    public string? ToUserId { get; set; }

    public string? ToMobileNumberId { get; set; }

    public string? ToUserSessionId { get; set; }

    public string? ToUserType { get; set; }

    public string? ToUserRole { get; set; }

    public string? TokenAmount { get; set; }

    public string? TokenTransferStatus { get; set; }

    public string? PaymentTicketId { get; set; }

    public string? PaymentTokenId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentTicketStatus { get; set; }

    public decimal? TokenPrice { get; set; }

    public decimal? TokenCost { get; set; }

    public string? SiteTokenTime { get; set; }

    public string? SiteTokenAmount { get; set; }

    public decimal? SiteTokenPrice { get; set; }

    public decimal? SiteTokenCost { get; set; }

    public string? TokenBalanceTime { get; set; }

    public string? TokenBalanceAmount { get; set; }

    public decimal? TokenBalancePrice { get; set; }

    public decimal? TokenBalanceCost { get; set; }

    public string? TokenTransaction { get; set; }

    public string? TokenBalance { get; set; }

    public bool? IsTokenCredit { get; set; }

    public int? TimeStartDay { get; set; }

    public int? TimeStartMonth { get; set; }

    public int? TimeStartYear { get; set; }

    public DateTime? TimeStartDate { get; set; }

    public int? TimeEndDay { get; set; }

    public int? TimeEndMonth { get; set; }

    public int? TimeEndYear { get; set; }

    public DateTime? TimeEndDate { get; set; }

    public int? LoginDay { get; set; }

    public int? LoginMonth { get; set; }

    public int? LoginYear { get; set; }

    public DateTime? LoginDate { get; set; }

    public int? LogoutDay { get; set; }

    public int? LogoutMonth { get; set; }

    public int? LogoutYear { get; set; }

    public DateTime? LogoutDate { get; set; }

    public DateTime? CancelDateTime { get; set; }

    public bool? IsNotificationSend { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
