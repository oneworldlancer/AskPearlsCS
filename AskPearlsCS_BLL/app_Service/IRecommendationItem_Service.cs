using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public interface IRecommendationItem_Service
    {
        Task<List<RecommendationItem>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default);
    }
}
