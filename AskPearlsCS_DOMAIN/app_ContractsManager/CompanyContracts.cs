namespace AskPearlsCS_DOMAIN.app_ContractsManager;

public sealed record CompanySaveRequest(
    string OrganizationTokenId,
    string CompanyName,
    string? CompanyWebsiteUrl = "0",
    string? CompanyLogoUrl = "0",
    string? CompanyIndustry = "0",
    string? CompanySize = "0",
    string? CompanyCountry = "0",
    string? CompanyCity = "0",
    string? EmailAddress = "0",
    string? PhoneNumber = "0");

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
