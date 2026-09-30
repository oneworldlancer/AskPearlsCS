using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DecisionOSManager
{
    public interface IDecisionEngine
    {
        DecisionResult Evaluate(
            IEnumerable<DecisionQuestion> questions,
            IEnumerable<DecisionRecommendation> recommendations,
            Dictionary<string, string> answers);
    }
}
