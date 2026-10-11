using AskPearls.Domain;

using AskPearlsCS_DAL.app_Repository;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.app_Service
{
    public class DecisionRule_Service : IDecisionRule_Service
    {
        private readonly IDecisionRule_Repository _decisionRuleRepository;

        public DecisionRule_Service(
            IDecisionRule_Repository decisionRuleRepository)
        {
            _decisionRuleRepository = decisionRuleRepository;
        }

        public Task<List<DecisionRule>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(widgetPublicKey) ||
                string.IsNullOrWhiteSpace(widgetTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return Task.FromResult(new List<DecisionRule>());
            }

            return _decisionRuleRepository.GetActiveByWidgetVersionAsync(
                widgetPublicKey,
                widgetTokenId,
                widgetVersionTokenId,
                cancellationToken);
        }
    }
}
