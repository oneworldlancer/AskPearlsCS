using System;
using System.Collections.Generic;

namespace AskPearlsCS_DAL.DBModels;

public partial class AskpearlsPortalApplicationDeviceLocationList
{
    public long DeviceLocationId { get; set; }

    public Guid RowViewTokenId { get; set; }

    public string? RowDataTokenId { get; set; }

    public string? DeviceGlobalId { get; set; }

    public string? AccountType { get; set; }

    public string? OwnerUserTokenId { get; set; }

    public string? OwnerMobileNumberTokenId { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? StateName { get; set; }

    public string? CityName { get; set; }

    public string? AddressLine { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? StreetLine { get; set; }

    public string? StreetLine1 { get; set; }

    public string? StreetLine2 { get; set; }

    public string? PostalCode { get; set; }

    public int? UploadDay { get; set; }

    public int? UploadMonth { get; set; }

    public int? UploadYear { get; set; }

    public DateTime? UploadDate { get; set; }

    public bool? IsDisplayToken { get; set; }

    public bool? IsActive { get; set; }
}
