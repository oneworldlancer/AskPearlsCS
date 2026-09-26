using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationRegisterInvitationList
{
    public long UregisterInviteId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DatabaseServerTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? WebmasterOwnerUserTokenId { get; set; }

    public string? WebmasterOwnerMobileNumberTokenId { get; set; }

    public string? RegisterInvitationTokenId { get; set; }

    public string? ApplicationRoleTokenId { get; set; }

    public string? ApplicationRoleCode { get; set; }

    public string? ReportCompany { get; set; }

    public string? EmployerCompany { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? CompanyName { get; set; }

    public string? AppRoleTokenId { get; set; }

    public string? AppRoleCode { get; set; }

    public string? AppRoleName { get; set; }

    public string? RoleTokenId { get; set; }

    public string? RoleCode { get; set; }

    public string? RoleName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? FullName { get; set; }

    public string? MobileNumberE164 { get; set; }

    public string? EmailAddress { get; set; }

    public string? Password { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? CityName { get; set; }

    public string? AddressLine { get; set; }

    public string? StreetLine { get; set; }

    public string? StreetLine1 { get; set; }

    public string? StreetLine2 { get; set; }

    public string? StreetLine3 { get; set; }

    public string? PostCode { get; set; }

    public string? PostCodeText { get; set; }

    public string? PostCodeValue { get; set; }

    public string? GmapLat { get; set; }

    public string? GmapLng { get; set; }

    public string? RegisterCompanyTokenId { get; set; }

    public string? InvitationCompanyTokenId { get; set; }

    public string? SupplierCompanyTokenId { get; set; }

    public bool? IsRegisterUserCompany { get; set; }

    public string? ActiveStatus { get; set; }

    public bool? IsRegister { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsActive { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
