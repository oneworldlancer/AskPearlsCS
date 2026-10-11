using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserOrganizationList
{
    public long UorganizationId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? OrganizationName { get; set; }

    public string? OrganizationCountry { get; set; }

    public string? OrganizationCity { get; set; }

    public string? OrganizationStreetName { get; set; }

    public string? OrganizationAddressLine { get; set; }

    public string? OrganizationAddress { get; set; }

    public string? OrganizationZipCode { get; set; }

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
