namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record CompanySaveRequest(
    string OrganizationTokenId,
    string CompanyName,
    string? CompanyWebsiteUrl = null,
    string? CompanyLogoUrl = null,
    string? CompanyIndustry = null,
    string? CompanySize = null,
    string? CompanyCountry = null,
    string? CompanyCity = null,
    string? EmailAddress = null,
    string? PhoneNumber = null);

public sealed record CompanyDto(
    string CompanyTokenId,
    string OrganizationTokenId,
    string CompanyName,
    string CompanyWebsiteUrl,
    string CompanyLogoUrl,
    string CompanyIndustry,
    string CompanySize,
    string CompanyCountry,
    string CompanyCity,
    string EmailAddress,
    string PhoneNumber);

public sealed record OrganizationSummaryDto(string OrganizationTokenId, string OrganizationName);
