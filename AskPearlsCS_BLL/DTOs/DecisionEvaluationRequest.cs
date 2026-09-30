using System;
using System.Collections.Generic;
using System.Text;

namespace AskPearlsCS_BLL.DTOs
{
    public class DecisionEvaluationRequest
    {
        public string WidgetKey { get; set; } = string.Empty;

        public Dictionary<string, string> Answers { get; set; }
            = new();
    }
}
