using AskPearlsCS_BLL.DecisionOSManager;
using AskPearlsCS_BLL.DTOs;

using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.WidgetDecision
{
    public interface IWidgetDecisionService
    {
        Task<DecisionResult> EvaluateAsync(
            DecisionEvaluationRequest request);
    }
}
