using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalUserLoginList
{
    public long UserLoginId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? UdeviceId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? OrganizationTokenId { get; set; }

    public string? CompanyTokenId { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? UserSessionId { get; set; }

    public string? DeviceId { get; set; }

    public string? AndroidId { get; set; }

    public string? Gcmid { get; set; }

    public string? CountryIsocode { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? CountryMobileCode { get; set; }

    public string? MobileNumber { get; set; }

    public string? MobileNumberLocal { get; set; }

    public string? MobileNumberInternational { get; set; }

    public string? MobileNumberE164 { get; set; }

    public string? MobileNumberRaw { get; set; }

    public string? MobileNumberOutUs { get; set; }

    public string? MobileNumberFormatUs { get; set; }

    public string? Line1Number { get; set; }

    public string? LoginStatus { get; set; }

    public string? VisibleStatus { get; set; }

    public DateTime? LastActivityDate { get; set; }

    public int? LastActivityDay { get; set; }

    public int? LastActivityMonth { get; set; }

    public int? LastActivityYear { get; set; }

    public bool? IsShowLastActivity { get; set; }

    public string? DeviceSoftwareVersion { get; set; }

    public string? NetworkCountryIso { get; set; }

    public string? GroupIdlevel1 { get; set; }

    public string? MmsUaprofUrl { get; set; }

    public string? MmsUserAgent { get; set; }

    public string? NetworkOperator { get; set; }

    public string? NetworkOperatorName { get; set; }

    public string? SimcountryIso { get; set; }

    public string? Simoperator { get; set; }

    public string? Simstate { get; set; }

    public string? SimserialNumber { get; set; }

    public string? SubscriberId { get; set; }

    public string? VoiceMailAlphaTag { get; set; }

    public string? VoiceMailNumber { get; set; }

    public string? CellLocation { get; set; }

    public string? NetworkType { get; set; }

    public string? DataActivity { get; set; }

    public string? DataState { get; set; }

    public string? NeighboringCellInfo { get; set; }

    public string? DeviceType { get; set; }

    public string? MobileType { get; set; }

    public string? PhoneType { get; set; }

    public string? AllCellInfo { get; set; }

    public string? CellIdentityGsm { get; set; }

    public bool? IsDeviceOnRegister { get; set; }

    public string? UploadDateTimeMilliSec { get; set; }

    public string? VisibleOnlineDateTimeMilliSec { get; set; }

    public bool? IsVisible { get; set; }

    public int? StatusUpdateDay { get; set; }

    public int? StatusUpdateMonth { get; set; }

    public int? StatusUpdateYear { get; set; }

    public DateTime? StatusUpdateDate { get; set; }

    public int? RemoveDay { get; set; }

    public int? RemoveMonth { get; set; }

    public int? RemoveYear { get; set; }

    public DateTime? RemoveDate { get; set; }

    public decimal? UploadDayUtc { get; set; }

    public int? UploadMonthUtc { get; set; }

    public int? UploadYearUtc { get; set; }

    public string? UploadTimeUtc { get; set; }

    public DateTime? UploadDateUtc { get; set; }

    public DateTime? UploadDateTimeUtc { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }
}
