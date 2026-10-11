using AskPearls.Domain;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{

    public interface IRecommendationItem_Repository
    {
        Task<List<RecommendationItem>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default);
    }
}
