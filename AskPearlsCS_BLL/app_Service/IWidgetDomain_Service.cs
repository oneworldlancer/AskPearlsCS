using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public interface IWidgetDomain_Service
{
    Task<WidgetOperationResult> GetDomainsAsync(string owner, string key);
    Task<WidgetOperationResult> AddDomainAsync(string owner, string key, string? domain);
    Task<WidgetOperationResult> DeleteDomainAsync(string owner, string key, string? domain);
    Task<WidgetOperationResult> BeginVerificationAsync(string owner, string key, string? domain);
    Task<WidgetOperationResult> VerifyDomainAsync(string owner, string key, string? domain);
}
