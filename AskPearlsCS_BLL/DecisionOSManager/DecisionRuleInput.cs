using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DecisionOSManager
{
    public class DecisionRuleInput
    {
        public string QuestionKey { get; set; } = string.Empty;

        public string AnswerValue { get; set; } = string.Empty;

        public int Score { get; set; }

        public string? Reason { get; set; }
    }
}
