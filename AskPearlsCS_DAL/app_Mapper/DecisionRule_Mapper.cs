using AskPearls.Domain;

using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_DAL.app_Mapper
{


    public static class DecisionRule_Mapper
    {
        public static DecisionRule ToDomain(
            AskpearlsPortalUserWebsiteWidgetDecisionRuleList source)
        {
            return new DecisionRule
            {
                Id = source.RowViewTokenId,

                DecisionRuleTokenId =
                    source.DecisionRuleTokenId ?? string.Empty,

                WidgetTokenId =
                    source.WidgetTokenId ?? string.Empty,

                WidgetVersionTokenId =
                    source.WidgetVersionTokenId ?? string.Empty,

                WidgetQuestionTokenId =
                    source.WidgetQuestionTokenId ?? string.Empty,

                WidgetQuestionKey =
                    source.WidgetQuestionKey ?? string.Empty,

                WidgetAnswerTokenId =
                    source.WidgetAnswerTokenId ?? string.Empty,

                WidgetAnswerKey =
                    source.WidgetAnswerKey ?? string.Empty,

                RecommendationTokenId =
                    source.RecommendationTokenId ?? string.Empty,

                RecommendationKey =
                    source.RecommendationKey ?? string.Empty,

                Score = source.DecisionRuleScore ?? 0,

                Reason = source.DecisionRuleReason ?? string.Empty,

                CreatedAtUtc =
                    source.CreatedAtUtc ?? DateTime.UtcNow,

                UpdatedAtUtc =
                    source.UpdatedAtUtc ?? DateTime.UtcNow
            };
        }
    }

}
