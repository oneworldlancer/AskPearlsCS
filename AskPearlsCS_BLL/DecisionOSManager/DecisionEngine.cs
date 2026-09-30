using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DecisionOSManager
{
    public class DecisionEngine : IDecisionEngine
    {
        private const int MaxScorePerQuestion = 50;

        public DecisionResult Evaluate(
            IEnumerable<DecisionQuestion> questions,
            IEnumerable<DecisionRecommendation> recommendations,
            Dictionary<string, string> answers)
        {
            var questionList = questions?.ToList()
                ?? new List<DecisionQuestion>();

            var recommendationList = recommendations?.ToList()
                ?? new List<DecisionRecommendation>();

            answers ??= new Dictionary<string, string>();

            var maximumScore =
                questionList.Count * MaxScorePerQuestion;

            var results = new List<RecommendationResult>();

            foreach (var recommendation in recommendationList)
            {
                var score = 0;

                var reasons = new List<string>();

                foreach (var rule in recommendation.Rules)
                {
                    if (string.IsNullOrWhiteSpace(rule.QuestionKey))
                        continue;

                    if (!answers.TryGetValue(
                            rule.QuestionKey,
                            out var visitorAnswer))
                    {
                        continue;
                    }

                    if (!string.Equals(
                            visitorAnswer,
                            rule.AnswerValue,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    score += rule.Score;

                    if (!string.IsNullOrWhiteSpace(rule.Reason))
                    {
                        reasons.Add(rule.Reason);
                    }
                }

                var percentage = 0;

                if (maximumScore > 0)
                {
                    percentage = (int)Math.Round(
                        (double)score / maximumScore * 100);
                }

                percentage = Math.Clamp(
                    percentage,
                    0,
                    100);

                results.Add(new RecommendationResult
                {
                    Key = recommendation.Key,
                    Name = recommendation.Name,

                    ImageUrl = recommendation.ImageUrl,
                    Url = recommendation.Url,

                    ShortDescription =
                        recommendation.ShortDescription,

                    Description =
                        recommendation.Description,

                    CtaText =
                        recommendation.CtaText,

                    Highlights =
                        recommendation.Highlights.ToList(),

                    Score = score,

                    MaximumScore = maximumScore,

                    Percentage = percentage,

                    Reasons = reasons
                });
            }

            var orderedResults = results
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Key)
                .ToList();

            return new DecisionResult
            {
                Success = true,

                MaximumScore = maximumScore,

                Recommendations = orderedResults,

                BestRecommendation =
                    orderedResults.FirstOrDefault()
            };
        }
    }
}
