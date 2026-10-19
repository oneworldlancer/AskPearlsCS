using AskPearlsCS_BLL.DTOs;
using AskPearlsCS_BLL.WidgetDecision;

using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Route("api/decisions")]
public class DecisionsController : ControllerBase
{
    private readonly IWidgetDecisionService _widgetDecisionService;

    public DecisionsController(IWidgetDecisionService widgetDecisionService)
    {
        _widgetDecisionService = widgetDecisionService;
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> Evaluate(
        [FromBody] DecisionEvaluationRequest request)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Request body is required."
            });
        }

        var result = await _widgetDecisionService.EvaluateAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}


//////////using AskPearlsCS_BLL.DecisionOSManager;
//////////using AskPearlsCS_BLL.DTOs;

//////////using Microsoft.AspNetCore.Mvc;

//////////namespace AskPearlsCS_API.Controllers
//////////{

//////////    [ApiController]
//////////    [Route("api/decisions")]
//////////    public class DecisionsController : ControllerBase
//////////    {
//////////        private readonly IDecisionEngine _decisionEngine;

//////////        public DecisionsController(IDecisionEngine decisionEngine)
//////////        {
//////////            _decisionEngine = decisionEngine;
//////////        }

//////////        [HttpPost("evaluate")]
//////////        public IActionResult Evaluate(
//////////            [FromBody] DecisionEvaluationRequest request)
//////////        {
//////////            if (request == null)
//////////            {
//////////                return BadRequest(new
//////////                {
//////////                    success = false,
//////////                    message = "Request body is required."
//////////                });
//////////            }

//////////            if (string.IsNullOrWhiteSpace(request.WidgetKey))
//////////            {
//////////                return BadRequest(new
//////////                {
//////////                    success = false,
//////////                    message = "WidgetKey is required."
//////////                });
//////////            }

//////////            if (request.Answers == null ||
//////////                request.Answers.Count == 0)
//////////            {
//////////                return BadRequest(new
//////////                {
//////////                    success = false,
//////////                    message = "At least one answer is required."
//////////                });
//////////            }

//////////            // ---------------------------------------------------------
//////////            // TEMPORARY DEMO DATA
//////////            // ---------------------------------------------------------
//////////            //
//////////            // This will later come from:
//////////            //
//////////            // Database
//////////            //   ↓
//////////            // WidgetVersion
//////////            //   ↓
//////////            // Questions
//////////            //   ↓
//////////            // RecommendationItems
//////////            //   ↓
//////////            // DecisionRules
//////////            //
//////////            // For now we use the existing Sofa Finder configuration.
//////////            // ---------------------------------------------------------

//////////            if (request.WidgetKey != "pk_demo_sofa_finder")
//////////            {
//////////                return NotFound(new
//////////                {
//////////                    success = false,
//////////                    message = "Widget not found."
//////////                });
//////////            }

//////////            var questions = new List<DecisionQuestion>
//////////        {
//////////            new()
//////////            {
//////////                Key = "users"
//////////            },

//////////            new()
//////////            {
//////////                Key = "priority"
//////////            }
//////////        };

//////////            var recommendations =
//////////                new List<DecisionRecommendation>
//////////                {
//////////                new()
//////////                {
//////////                    Key = "oslo",
//////////                    Name = "Oslo Modular Sofa",

//////////                    ImageUrl =
//////////                        "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

//////////                    Url =
//////////                        "https://example.com/products/oslo-modular-sofa",

//////////                    ShortDescription =
//////////                        "A spacious, comfortable modular sofa designed for modern family living.",

//////////                    Description =
//////////                        "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

//////////                    CtaText = "View Oslo Sofa",

//////////                    Highlights =
//////////                    [
//////////                        "Seats up to 6 people",
//////////                        "Modular configuration",
//////////                        "Deep comfortable seating",
//////////                        "Modern neutral design"
//////////                    ],

//////////                    Rules =
//////////                    [
//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "small",
//////////                            Score = 15,
//////////                            Reason =
//////////                                "Works well for smaller households"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "medium",
//////////                            Score = 25,
//////////                            Reason =
//////////                                "Comfortable for families and small groups"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "large",
//////////                            Score = 40,
//////////                            Reason =
//////////                                "Great for larger groups"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "comfort",
//////////                            Score = 35,
//////////                            Reason =
//////////                                "Comfort-focused design"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "style",
//////////                            Score = 10,
//////////                            Reason =
//////////                                "Modern and elegant appearance"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "space",
//////////                            Score = 15,
//////////                            Reason =
//////////                                "Flexible modular configuration"
//////////                        }
//////////                    ]
//////////                },

//////////                new()
//////////                {
//////////                    Key = "luna",
//////////                    Name = "Luna Designer Sofa",

//////////                    ImageUrl =
//////////                        "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

//////////                    Url =
//////////                        "https://example.com/products/luna-designer-sofa",

//////////                    ShortDescription =
//////////                        "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

//////////                    Description =
//////////                        "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

//////////                    CtaText = "View Luna Sofa",

//////////                    Highlights =
//////////                    [
//////////                        "Contemporary design",
//////////                        "Space-efficient",
//////////                        "Premium upholstery",
//////////                        "Multiple color options"
//////////                    ],

//////////                    Rules =
//////////                    [
//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "small",
//////////                            Score = 35,
//////////                            Reason =
//////////                                "Ideal for smaller spaces"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "medium",
//////////                            Score = 25,
//////////                            Reason =
//////////                                "Works well for everyday living"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "users",
//////////                            AnswerValue = "large",
//////////                            Score = 10,
//////////                            Reason =
//////////                                "Can accommodate larger households"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "comfort",
//////////                            Score = 15,
//////////                            Reason =
//////////                                "Comfortable premium seating"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "style",
//////////                            Score = 40,
//////////                            Reason =
//////////                                "Elegant contemporary design"
//////////                        },

//////////                        new()
//////////                        {
//////////                            QuestionKey = "priority",
//////////                            AnswerValue = "space",
//////////                            Score = 35,
//////////                            Reason =
//////////                                "Space-efficient footprint"
//////////                        }
//////////                    ]
//////////                }
//////////                };

//////////            // ---------------------------------------------------------
//////////            // RUN DECISION ENGINE
//////////            // ---------------------------------------------------------

//////////            var result = _decisionEngine.Evaluate(
//////////                questions,
//////////                recommendations,
//////////                request.Answers);

//////////            return Ok(result);
//////////        }

//////////    }

//////////}