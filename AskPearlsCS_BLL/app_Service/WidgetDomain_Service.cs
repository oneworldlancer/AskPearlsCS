using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;
namespace AskPearlsCS_BLL.app_Service;

public sealed class WidgetDomain_Service(IWidgetDomain_Repository repository) : IWidgetDomain_Service
{
    public Task<WidgetOperationResult> GetDomainsAsync(string owner, string key) => repository.GetDomainsAsync(owner, key);
    public Task<WidgetOperationResult> AddDomainAsync(string owner, string key, string? domain) => repository.AddDomainAsync(owner, key, domain);
    public Task<WidgetOperationResult> DeleteDomainAsync(string owner, string key, string? domain) => repository.DeleteDomainAsync(owner, key, domain);
    public Task<WidgetOperationResult> BeginVerificationAsync(string owner, string key, string? domain) => repository.BeginVerificationAsync(owner, key, domain);
    public Task<WidgetOperationResult> VerifyDomainAsync(string owner, string key, string? domain) => repository.VerifyDomainAsync(owner, key, domain);
}
