using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DecisionOSManager
{
    public class DecisionResult
    {
        public bool Success { get; set; }

        public int MaximumScore { get; set; }

        public List<RecommendationResult> Recommendations { get; set; }
            = new();

        public RecommendationResult? BestRecommendation { get; set; }
    }
}
