using AskPearlsCS_BLL.app_Service;
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
        private readonly IWidgetVersion_Service _widgetVersionService;
        private readonly IQuestion_Service _questionService;
        private readonly IAnswer_Service _answerService;
        private readonly IRecommendationItem_Service _recommendationService;
        private readonly IDecisionRule_Service _decisionRuleService;
        private readonly IDecisionEngine _decisionEngine;

        public WidgetDecisionService(
            IWidgetVersion_Service widgetVersionService,
            IQuestion_Service questionService,
            IAnswer_Service answerService,
            IRecommendationItem_Service recommendationService,
            IDecisionRule_Service decisionRuleService,
            IDecisionEngine decisionEngine)
        {
            _widgetVersionService = widgetVersionService;
            _questionService = questionService;
            _answerService = answerService;
            _recommendationService = recommendationService;
            _decisionRuleService = decisionRuleService;
            _decisionEngine = decisionEngine;
        }

        public async Task<DecisionResult> EvaluateAsync(
            DecisionEvaluationRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.WidgetKey) ||
                request.Answers == null ||
                request.Answers.Count == 0)
            {
                return Failure();
            }

            // 1. Resolve the published widget version.
            var version = await _widgetVersionService
                .GetPublishedVersionAsync(request.WidgetKey);

            if (version == null ||
                string.IsNullOrWhiteSpace(version.VersionTokenId))
            {
                return Failure();
            }

            // 2. Load the published questions.
            var questions = await _questionService
                .GetPublishedQuestionsAsync(request.WidgetKey);

            if (questions == null || questions.Count == 0)
            {
                return Failure();
            }

            // 3. Validate every submitted question and answer.
            // Also require an answer for every active published question.
            var questionKeys = questions
                .Select(q => q.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (request.Answers.Keys.Any(
                key => !questionKeys.Contains(key)))
            {
                return Failure();
            }

            var validatedAnswers =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var question in questions)
            {
                if (!request.Answers.TryGetValue(
                    question.Key, out var submittedAnswer) ||
                    string.IsNullOrWhiteSpace(submittedAnswer))
                {
                    return Failure();
                }

                var activeAnswers = await _answerService
                    .GetActiveByQuestionAsync(
                        request.WidgetKey,
                        question.QuestionTokenId,
                        version.VersionTokenId);

                var matchedAnswer = activeAnswers.FirstOrDefault(
                    answer => string.Equals(
                        answer.Key,
                        submittedAnswer,
                        StringComparison.OrdinalIgnoreCase));

                if (matchedAnswer == null)
                {
                    return Failure();
                }

                validatedAnswers[question.Key] = matchedAnswer.Key;
            }

            // The repositories require the widget token as well as its
            // public key and published-version token.
            var widgetTokenId = questions
                .Select(q => q.WidgetTokenId)
                .FirstOrDefault(
                    token => !string.IsNullOrWhiteSpace(token));

            if (string.IsNullOrWhiteSpace(widgetTokenId))
            {
                return Failure();
            }

            // 4. Load recommendations and decision rules for this version.
            var recommendationItems = await _recommendationService
                .GetActiveByWidgetVersionAsync(
                    request.WidgetKey,
                    widgetTokenId,
                    version.VersionTokenId);

            var decisionRules = await _decisionRuleService
                .GetActiveByWidgetVersionAsync(
                    request.WidgetKey,
                    widgetTokenId,
                    version.VersionTokenId);

            if (recommendationItems.Count == 0 ||
                decisionRules.Count == 0)
            {
                return Failure();
            }

            // 5. Convert published questions to engine inputs.
            var decisionQuestions = questions
                .Select(question => new DecisionQuestion
                {
                    Key = question.Key
                })
                .ToList();

            // 6. Build engine recommendations with their scoring rules.
            var decisionRecommendations = recommendationItems
                .Select(recommendation =>
                {
                    var rules = decisionRules
                        .Where(rule =>
                            string.Equals(
                                rule.RecommendationKey,
                                recommendation.Key,
                                StringComparison.OrdinalIgnoreCase))
                        .Select(rule => new DecisionRuleInput
                        {
                            QuestionKey = rule.WidgetQuestionKey,
                            AnswerValue = rule.WidgetAnswerKey,
                            Score = rule.Score,
                            Reason = rule.Reason
                        })
                        .Where(rule =>
                            !string.IsNullOrWhiteSpace(rule.QuestionKey) &&
                            !string.IsNullOrWhiteSpace(rule.AnswerValue))
                        .ToList();

                    return new DecisionRecommendation
                    {
                        Key = recommendation.Key,
                        Name = recommendation.Name,
                        ImageUrl = recommendation.ImageUrl,
                        Url = recommendation.Url,
                        ShortDescription = recommendation.ShortDescription,
                        Description = recommendation.Description,
                        CtaText = recommendation.CtaText,
                        Highlights = recommendation.Highlights ?? new List<string>(),
                        Rules = rules
                    };
                })
                .ToList();

            if (decisionRecommendations.Any(
                recommendation => recommendation.Rules.Count == 0))
            {
                // Do not silently evaluate a recommendation without rules.
                return Failure();
            }

            // 7. Run the existing DecisionOS scoring engine.
            return _decisionEngine.Evaluate(
                decisionQuestions,
                decisionRecommendations,
                validatedAnswers);
        }

        private static DecisionResult Failure()
        {
            return new DecisionResult
            {
                Success = false,
                MaximumScore = 0,
                Recommendations = new(),
                BestRecommendation = null
            };
        }
    }
}
