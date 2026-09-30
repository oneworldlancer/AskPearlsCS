using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DecisionOSManager
{
    public class RecommendationResult
    {
        public string Key { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? Url { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? CtaText { get; set; }

        public List<string> Highlights { get; set; }
            = new();

        public int Score { get; set; }

        public int MaximumScore { get; set; }

        public int Percentage { get; set; }

        public List<string> Reasons { get; set; }
            = new();
    }
}
