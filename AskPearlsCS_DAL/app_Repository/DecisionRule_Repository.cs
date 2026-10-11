using AskPearls.Domain;

using AskPearlsCS_DAL.app_Mapper;
using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Repository
{


    public class DecisionRule_Repository : IDecisionRule_Repository
    {
        private readonly DBContextModel _context;

        public DecisionRule_Repository(DBContextModel context)
        {
            _context = context;
        }

        public async Task<List<DecisionRule>> GetActiveByWidgetVersionAsync(
            string widgetPublicKey,
            string widgetTokenId,
            string widgetVersionTokenId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(widgetPublicKey) ||
                string.IsNullOrWhiteSpace(widgetTokenId) ||
                string.IsNullOrWhiteSpace(widgetVersionTokenId))
            {
                return new List<DecisionRule>();
            }

            var rows = await _context
                .AskpearlsPortalUserWebsiteWidgetDecisionRuleLists
                .AsNoTracking()
                .Where(x =>
                    x.WidgetPublicKey == widgetPublicKey &&
                    x.WidgetTokenId == widgetTokenId &&
                    x.WidgetVersionTokenId == widgetVersionTokenId &&
                    x.IsActive == true &&
                    x.IsVisible == true &&
                    x.DecisionRuleScore != null &&
                    x.RecommendationKey != null &&
                    x.WidgetQuestionKey != null &&
                    x.WidgetAnswerKey != null)
                .OrderBy(x => x.WidgetQuestionKey)
                .ThenBy(x => x.WidgetAnswerKey)
                .ThenBy(x => x.RecommendationKey)
                .ToListAsync(cancellationToken);

            return rows
                .Select(DecisionRule_Mapper.ToDomain)
                .ToList();
        }
    }


}
