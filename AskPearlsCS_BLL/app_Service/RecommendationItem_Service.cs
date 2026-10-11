using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{

    public class RecommendationItem_Service : IRecommendationItem_Service
    {
        private readonly IRecommendationItem_Repository _repository;

        public RecommendationItem_Service(
            IRecommendationItem_Repository repository)
        {
            _repository = repository;
        }

        public Task<List<RecommendationItem>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default)
        {
            return _repository.GetActiveByWidgetVersionAsync(
                widgetPublicKey,
                widgetTokenId,
                widgetVersionTokenId,
                cancellationToken);
        }
    }
}
