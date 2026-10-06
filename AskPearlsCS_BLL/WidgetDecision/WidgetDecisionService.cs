using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.DTOs;

using AskPearlsCS_DAL.DBContext;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.WidgetDecision
{

    public class WidgetDecisionService : IWidgetDecisionService
    {
        private readonly DBContextModel _dbContext;
        private readonly IDecisionEngine _decisionEngine;

        public WidgetDecisionService(
            DBContextModel dbContext,
            IDecisionEngine decisionEngine)
        {
            _dbContext = dbContext;
            _decisionEngine = decisionEngine;
        }

        public async Task<DecisionResult> EvaluateAsync(
            DecisionEvaluationRequest request)
        {
            return null;

            //////////// ---------------------------------------------------------
            //////////// 1. Find widget
            //////////// ---------------------------------------------------------

            //////////var widget = await _dbContext.AskpearlsPortalUserWebsiteWidgetLists
            //////////    .AsNoTracking()
            //////////    .FirstOrDefaultAsync(x =>
            //////////        x.WidgetPublicKey == request.WidgetKey &&
            //////////        x.WidgetStatus  == "Published");

            //////////if (widget == null)
            //////////{
            //////////    return new DecisionResult
            //////////    {
            //////////        Success = false
            //////////    };
            //////////}

            //////////// ---------------------------------------------------------
            //////////// 2. Find published widget version
            //////////// ---------------------------------------------------------

            //////////var widgetVersion = await _dbContext.AskpearlsPortalUserWebsiteWidgetVersionLists
            //////////    .AsNoTracking()
            //////////    .FirstOrDefaultAsync(x =>
            //////////        x.WidgetTokenId == widget.WidgetTokenId &&
            //////////        x.WidgetPublishedVersion == widget.WidgetPublishedVersion &&
            //////////        x.WidgetStatus == "Published");

            //////////if (widgetVersion == null)
            //////////{
            //////////    return new DecisionResult
            //////////    {
            //////////        Success = false
            //////////    };
            //////////}

            //////////// ---------------------------------------------------------
            //////////// 3. Load questions
            //////////// ---------------------------------------------------------

            //////////var questions = await _dbContext.AskpearlsPortalUserwidgetQuestionLists
            //////////    .AsNoTracking()
            //////////    .Where(x =>
            //////////        x.WidgetVersionId == widgetVersion.Id)
            //////////    .OrderBy(x => x.SortOrder)
            //////////    .ToListAsync();

            //////////// ---------------------------------------------------------
            //////////// 4. Load recommendation items
            //////////// ---------------------------------------------------------

            //////////var recommendationItems =
            //////////    await _dbContext.RecommendationItems
            //////////        .AsNoTracking()
            //////////        .Where(x =>
            //////////            x.WidgetVersionId == widgetVersion.Id &&
            //////////            x.IsActive)
            //////////        .OrderBy(x => x.SortOrder)
            //////////        .ToListAsync();

            //////////// ---------------------------------------------------------
            //////////// 5. Load decision rules
            //////////// ---------------------------------------------------------

            //////////var decisionRules =
            //////////    await _dbContext.DecisionRules
            //////////        .AsNoTracking()
            //////////        .Where(x =>
            //////////            x.WidgetVersionId == widgetVersion.Id)
            //////////        .ToListAsync();

            //////////// ---------------------------------------------------------
            //////////// 6. Convert DB questions → DecisionOS questions
            //////////// ---------------------------------------------------------

            //////////var decisionQuestions =
            //////////    questions
            //////////        .Select(x => new DecisionQuestion
            //////////        {
            //////////            Key = x.Key
            //////////        })
            //////////        .ToList();

            //////////// ---------------------------------------------------------
            //////////// 7. Convert DB recommendations → DecisionOS
            //////////// ---------------------------------------------------------

            //////////var decisionRecommendations =
            //////////    recommendationItems
            //////////        .Select(recommendation =>
            //////////        {
            //////////            var rules = decisionRules
            //////////                .Where(rule =>
            //////////                    rule.RecommendationItemId ==
            //////////                    recommendation.Id)
            //////////                .Select(rule =>
            //////////                {
            //////////                    var question =
            //////////                        questions.FirstOrDefault(q =>
            //////////                            q.Id == rule.QuestionId);

            //////////                    var answer =
            //////////                        question == null
            //////////                            ? null
            //////////                            : _dbContext.Answers
            //////////                                .AsNoTracking()
            //////////                                .FirstOrDefault(a =>
            //////////                                    a.Id == rule.AnswerId);

            //////////                    return new DecisionRuleInput
            //////////                    {
            //////////                        QuestionKey =
            //////////                            question?.Key ?? string.Empty,

            //////////                        AnswerValue =
            //////////                            answer?.Value ?? string.Empty,

            //////////                        Score =
            //////////                            rule.Score,

            //////////                        Reason =
            //////////                            rule.Reason
            //////////                    };
            //////////                })
            //////////                .Where(x =>
            //////////                    !string.IsNullOrWhiteSpace(
            //////////                        x.QuestionKey))
            //////////                .ToList();

            //////////            return new DecisionRecommendation
            //////////            {
            //////////                Key = recommendation.Key,

            //////////                Name = recommendation.Name,

            //////////                ImageUrl =
            //////////                    recommendation.ImageUrl,

            //////////                Url =
            //////////                    recommendation.Url,

            //////////                ShortDescription =
            //////////                    recommendation.ShortDescription,

            //////////                Description =
            //////////                    recommendation.Description,

            //////////                CtaText =
            //////////                    recommendation.CtaText,

            //////////                Highlights =
            //////////                    ParseHighlights(
            //////////                        recommendation.HighlightsJson),

            //////////                Rules = rules
            //////////            };
            //////////        })
            //////////        .ToList();

            //////////// ---------------------------------------------------------
            //////////// 8. Run DecisionOS
            //////////// ---------------------------------------------------------

            //////////return _decisionEngine.Evaluate(
            //////////    decisionQuestions,
            //////////    decisionRecommendations,
            //////////    request.Answers);
        }

        private static List<string> ParseHighlights(
            string? highlightsJson)
        {
            if (string.IsNullOrWhiteSpace(highlightsJson))
                return new List<string>();

            try
            {
                return System.Text.Json.JsonSerializer
                    .Deserialize<List<string>>(highlightsJson)
                    ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
