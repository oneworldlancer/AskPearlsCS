using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public interface IDecisionRule_Service
    {
        Task<List<DecisionRule>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default);
    }
}
