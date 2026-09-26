using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserCompanyList
{
    public long UcompanyId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyCountry { get; set; }

    public string? CompanyCity { get; set; }

    public string? CompanyStreetName { get; set; }

    public string? CompanyAddressLine { get; set; }

    public string? CompanyAddress { get; set; }

    public string? CompanyZipCode { get; set; }

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CountryName { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNumberCode { get; set; }

    public string? MobileNumber { get; set; }

    public string? MobileNumberE164 { get; set; }

    public string? MobileNumberWhatsappE164 { get; set; }

    public bool? IsValidatePhoneNumber { get; set; }

    public bool? IsVerifiedPhoneNumber { get; set; }

    public bool? IsValidateMobileNumber { get; set; }

    public bool? IsVerifiedMobileNumber { get; set; }

    public bool? IsValidateMobileNumberE164 { get; set; }

    public bool? IsVerifiedMobileNumberE164 { get; set; }

    public bool? IsValidateMobileNumberWhatsappE164 { get; set; }

    public bool? IsVerifiedMobileNumberWhatsappE164 { get; set; }

    public int? ItemOrder { get; set; }

    public bool? IsVisible { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
