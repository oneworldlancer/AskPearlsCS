using AskPearlsCS_DAL.app_Repository;

using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_BLL.app_Service;


public sealed class WidgetDomain_Service : IWidgetDomain_Service
{
    private readonly IWidgetDomain_Repository _repository;
    public WidgetDomain_Service(IWidgetDomain_Repository repository) => _repository = repository;
    public Task<WidgetOperationResult> GetDomainsAsync(string owner, string key) => _repository.GetDomainsAsync(owner, key);
    public Task<WidgetOperationResult> AddDomainAsync(string owner, string key, string? domain) => _repository.AddDomainAsync(owner, key, domain);
    public Task<WidgetOperationResult> DeleteDomainAsync(string owner, string key, string? domain) => _repository.DeleteDomainAsync(owner, key, domain);
}
