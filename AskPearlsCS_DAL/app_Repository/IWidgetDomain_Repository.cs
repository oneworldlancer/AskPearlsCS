using System;
using System.Collections.Generic;
using System.Text;
using AskPearlsCS_DOMAIN.app_ContractsManager;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IWidgetDomain_Repository
    {
        Task<WidgetOperationResult> GetDomainsAsync(string owner, string publicKey);
        Task<WidgetOperationResult> AddDomainAsync(string owner, string publicKey, string? domain);
        Task<WidgetOperationResult> DeleteDomainAsync(string owner, string publicKey, string? domain);
    }

}
