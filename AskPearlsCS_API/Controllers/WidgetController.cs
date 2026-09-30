//using AskPearls.Infrastructure;
using AskPearlsCS_BLL.DecisionOSManager;

using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AskPearlsCS_API.Controllers;

[ApiController]
[Route("api/widgets")]
public class WidgetController : ControllerBase
//public class WidgetController(AskPearlsDbContext db) : ControllerBase
{
    [HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished(string publicKey)
    {
        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();




//////////        var questions = new List<DecisionQuestion>
//////////{
//////////    new()
//////////    {
//////////        Key = "users"
//////////    },

//////////    new()
//////////    {
//////////        Key = "priority"
//////////    }
//////////};

//////////        var recommendations = new List<DecisionRecommendation>
//////////{
//////////    new()
//////////    {
//////////        Key = "oslo",
//////////        Name = "Oslo Modular Sofa",

//////////        Rules =
//////////        [
//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "small",
//////////                Score = 15,
//////////                Reason = "Works well for smaller households"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "medium",
//////////                Score = 25,
//////////                Reason = "Comfortable for families and small groups"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "large",
//////////                Score = 40,
//////////                Reason = "Great for larger groups"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "comfort",
//////////                Score = 35,
//////////                Reason = "Comfort-focused design"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "style",
//////////                Score = 10,
//////////                Reason = "Modern and elegant appearance"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "space",
//////////                Score = 15,
//////////                Reason = "Flexible modular configuration"
//////////            }
//////////        ]
//////////    },

//////////    new()
//////////    {
//////////        Key = "luna",
//////////        Name = "Luna Designer Sofa",

//////////        Rules =
//////////        [
//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "small",
//////////                Score = 35,
//////////                Reason = "Ideal for smaller spaces"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "medium",
//////////                Score = 25,
//////////                Reason = "Works well for everyday living"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "users",
//////////                AnswerValue = "large",
//////////                Score = 10,
//////////                Reason = "Can accommodate larger households"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "comfort",
//////////                Score = 15,
//////////                Reason = "Comfortable premium seating"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "style",
//////////                Score = 40,
//////////                Reason = "Elegant contemporary design"
//////////            },

//////////            new()
//////////            {
//////////                QuestionKey = "priority",
//////////                AnswerValue = "space",
//////////                Score = 35,
//////////                Reason = "Space-efficient footprint"
//////////            }
//////////        ]
//////////    }
//////////};


//////////        var answers = new Dictionary<string, string>
//////////        {
//////////            ["users"] = "large",
//////////            ["priority"] = "comfort"
//////////        };

//////////        var engine = new DecisionEngine();

//////////        var result = engine.Evaluate(
//////////            questions,
//////////            recommendations,
//////////            answers);





        /////////////////////////


        var configuration = new
        {
            /*
             * =========================================================
             * WIDGET
             * =========================================================
             */

            widget = new
            {
                key = "pk_demo_sofa_finder",

                name = "Sofa Finder",

                title = "Find Your Perfect Sofa",

                subtitle =
                    "Answer a few questions and we'll find the sofa that fits you best."
            },


            /*
             * =========================================================
             * QUESTIONS
             * =========================================================
             */

            questions = new[]
            {
            /*
             * =====================================================
             * QUESTION 1
             * =====================================================
             */

            new
            {
                key = "users",

                text =
                    "How many people will use the sofa?",

                options = new[]
                {
                    new
                    {
                        text = "1–2 people",

                        value = "small"
                    },

                    new
                    {
                        text = "3–4 people",

                        value = "medium"
                    },

                    new
                    {
                        text = "5+ people",

                        value = "large"
                    }
                }
            },


            /*
             * =====================================================
             * QUESTION 2
             * =====================================================
             */

            new
            {
                key = "priority",

                text =
                    "What matters most to you?",

                options = new[]
                {
                    new
                    {
                        text = "Comfort",

                        value = "comfort"
                    },

                    new
                    {
                        text = "Style",

                        value = "style"
                    },

                    new
                    {
                        text = "Space saving",

                        value = "space"
                    }
                }
            }
        },


            /*
             * =========================================================
             * RECOMMENDATION ITEMS
             *
             * These are the possible final results.
             *
             * The scores are NOT stored here directly.
             *
             * Scores are defined by the Decision Rules below.
             * =========================================================
             */

            recommendations = new[]
            {
            /*
             * =====================================================
             * RECOMMENDATION 1
             * OSLO
             * =====================================================
             */

            new
            {
                key = "oslo",

                name =
                    "Oslo Modular Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

                url =
                    "https://example.com/products/oslo-modular-sofa",

                shortDescription =
                    "A spacious, comfortable modular sofa designed for modern family living.",

                description =
                    "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

                ctaText =
                    "View Oslo Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Seats up to 6 people",

                    "Modular configuration",

                    "Deep comfortable seating",

                    "Modern neutral design"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 *
                 * IMPORTANT:
                 *
                 * For EVERY possible answer:
                 *
                 * Oslo Score + Luna Score = 50
                 *
                 * Example:
                 *
                 * 5+ people
                 *
                 * Oslo = 40
                 * Luna = 10
                 *
                 * Total = 50
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            15,

                        reason =
                            "Works well for smaller households"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Comfortable for families and small groups"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            40,

                        reason =
                            "Great for larger groups"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            35,

                        reason =
                            "Comfort-focused design"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            10,

                        reason =
                            "Modern and elegant appearance"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            15,

                        reason =
                            "Flexible modular configuration"
                    }
                }
            },


            /*
             * =====================================================
             * RECOMMENDATION 2
             * LUNA
             * =====================================================
             */

            new
            {
                key = "luna",

                name =
                    "Luna Designer Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

                url =
                    "https://example.com/products/luna-designer-sofa",

                shortDescription =
                    "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

                description =
                    "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

                ctaText =
                    "View Luna Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Contemporary design",

                    "Space-efficient",

                    "Premium upholstery",

                    "Multiple color options"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            35,

                        reason =
                            "Ideal for smaller spaces"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     * Luna = 25
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Works well for everyday living"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     * Luna = 10
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            10,

                        reason =
                            "Can accommodate larger households"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     * Luna = 15
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            15,

                        reason =
                            "Comfortable premium seating"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     * Luna = 40
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            40,

                        reason =
                            "Elegant contemporary design"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            35,

                        reason =
                            "Space-efficient footprint"
                    }
                }
            }
        }
        };


        return Ok(configuration);

    }



//    [HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished_X2(string publicKey)
    {
        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();


        var configuration = new
        {
            /*
             * =========================================================
             * WIDGET
             * =========================================================
             */

            widget = new
            {
                key = "pk_demo_sofa_finder",

                name = "Sofa Finder",

                title = "Find Your Perfect Sofa",

                subtitle =
                    "Answer a few questions and we'll find the sofa that fits you best."
            },


            /*
             * =========================================================
             * QUESTIONS
             * =========================================================
             */

            questions = new[]
            {
            /*
             * =====================================================
             * QUESTION 1
             * =====================================================
             */

            new
            {
                key = "users",

                text =
                    "How many people will use the sofa?",

                options = new[]
                {
                    new
                    {
                        text = "1–2 people",

                        value = "small"
                    },

                    new
                    {
                        text = "3–4 people",

                        value = "medium"
                    },

                    new
                    {
                        text = "5+ people",

                        value = "large"
                    }
                }
            },


            /*
             * =====================================================
             * QUESTION 2
             * =====================================================
             */

            new
            {
                key = "priority",

                text =
                    "What matters most to you?",

                options = new[]
                {
                    new
                    {
                        text = "Comfort",

                        value = "comfort"
                    },

                    new
                    {
                        text = "Style",

                        value = "style"
                    },

                    new
                    {
                        text = "Space saving",

                        value = "space"
                    }
                }
            }
        },


            /*
             * =========================================================
             * RECOMMENDATION ITEMS
             *
             * These are the possible final results.
             *
             * The scores are NOT stored here directly.
             *
             * Scores are defined by the Decision Rules below.
             * =========================================================
             */

            recommendations = new[]
            {
            /*
             * =====================================================
             * RECOMMENDATION 1
             * OSLO
             * =====================================================
             */

            new
            {
                key = "oslo",

                name =
                    "Oslo Modular Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

                url =
                    "https://example.com/products/oslo-modular-sofa",

                shortDescription =
                    "A spacious, comfortable modular sofa designed for modern family living.",

                description =
                    "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

                ctaText =
                    "View Oslo Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Seats up to 6 people",

                    "Modular configuration",

                    "Deep comfortable seating",

                    "Modern neutral design"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 *
                 * IMPORTANT:
                 *
                 * For EVERY possible answer:
                 *
                 * Oslo Score + Luna Score = 50
                 *
                 * Example:
                 *
                 * 5+ people
                 *
                 * Oslo = 40
                 * Luna = 10
                 *
                 * Total = 50
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            15,

                        reason =
                            "Works well for smaller households"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Comfortable for families and small groups"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            40,

                        reason =
                            "Great for larger groups"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            35,

                        reason =
                            "Comfort-focused design"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            10,

                        reason =
                            "Modern and elegant appearance"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            15,

                        reason =
                            "Flexible modular configuration"
                    }
                }
            },


            /*
             * =====================================================
             * RECOMMENDATION 2
             * LUNA
             * =====================================================
             */

            new
            {
                key = "luna",

                name =
                    "Luna Designer Sofa",

                imageUrl =
                    "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

                url =
                    "https://example.com/products/luna-designer-sofa",

                shortDescription =
                    "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

                description =
                    "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

                ctaText =
                    "View Luna Sofa",


                /*
                 * -------------------------------------------------
                 * PRODUCT HIGHLIGHTS
                 * -------------------------------------------------
                 */

                highlights = new[]
                {
                    "Contemporary design",

                    "Space-efficient",

                    "Premium upholstery",

                    "Multiple color options"
                },


                /*
                 * -------------------------------------------------
                 * DECISION RULES
                 * -------------------------------------------------
                 */

                rules = new[]
                {
                    /*
                     * =================================================
                     * QUESTION 1
                     * How many people will use the sofa?
                     * =================================================
                     */


                    /*
                     * 1–2 people
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "small",

                        score =
                            35,

                        reason =
                            "Ideal for smaller spaces"
                    },


                    /*
                     * 3–4 people
                     *
                     * Oslo = 25
                     * Luna = 25
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "medium",

                        score =
                            25,

                        reason =
                            "Works well for everyday living"
                    },


                    /*
                     * 5+ people
                     *
                     * Oslo = 40
                     * Luna = 10
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "users",

                        answerValue =
                            "large",

                        score =
                            10,

                        reason =
                            "Can accommodate larger households"
                    },


                    /*
                     * =================================================
                     * QUESTION 2
                     * What matters most to you?
                     * =================================================
                     */


                    /*
                     * Comfort
                     *
                     * Oslo = 35
                     * Luna = 15
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "comfort",

                        score =
                            15,

                        reason =
                            "Comfortable premium seating"
                    },


                    /*
                     * Style
                     *
                     * Oslo = 10
                     * Luna = 40
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "style",

                        score =
                            40,

                        reason =
                            "Elegant contemporary design"
                    },


                    /*
                     * Space saving
                     *
                     * Oslo = 15
                     * Luna = 35
                     *
                     * Total = 50
                     */

                    new
                    {
                        questionKey =
                            "priority",

                        answerValue =
                            "space",

                        score =
                            35,

                        reason =
                            "Space-efficient footprint"
                    }
                }
            }
        }
        };


        return Ok(configuration);

    }

    ////[HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished_X1(string publicKey)
    {

        if (publicKey != "pk_demo_sofa_finder")
            return NotFound();

        var configuration = new
        {
            title = "Find Your Perfect Sofa",

            subtitle = "Answer a few questions and we'll find the sofa that fits you best.",

            questions = new[]
            {
                new
                {
                    key = "users",

                    text = "How many people will use the sofa?",

                    options = new[]
                    {
                        new
                        {
                            text = "1–2 people",
                            value = "small"
                        },

                        new
                        {
                            text = "3–4 people",
                            value = "medium"
                        },

                        new
                        {
                            text = "5+ people",
                            value = "large"
                        }
                    }
                },

                new
                {
                    key = "priority",

                    text = "What matters most to you?",

                    options = new[]
                    {
                        new
                        {
                            text = "Comfort",
                            value = "comfort"
                        },

                        new
                        {
                            text = "Style",
                            value = "style"
                        },

                        new
                        {
                            text = "Space saving",
                            value = "space"
                        }
                    }
                }
            },

            recommendations = new[]
{
    new
    {
        key = "oslo",

        name = "Oslo Modular Sofa",

        imageUrl =
            "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

        url =
            "https://example.com/products/oslo-modular-sofa",

        shortDescription =
            "A spacious, comfortable modular sofa designed for modern family living.",

        description =
            "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

        ctaText = "View Oslo Sofa",

        /*
         * DecisionOS scoring rules
         */
        rules = new[]
        {
            new
            {
                questionKey = "users",
                answerValue = "large",
                score = 40,
                reason = "Great for larger groups"
            },

            new
            {
                questionKey = "users",
                answerValue = "medium",
                score = 25,
                reason = "Comfortable for families and small groups"
            },

            new
            {
                questionKey = "users",
                answerValue = "small",
                score = 10,
                reason = "Works well for smaller households"
            },

            new
            {
                questionKey = "priority",
                answerValue = "comfort",
                score = 45,
                reason = "Comfort-focused design"
            },

            new
            {
                questionKey = "priority",
                answerValue = "style",
                score = 10,
                reason = "Modern and elegant appearance"
            },

            new
            {
                questionKey = "priority",
                answerValue = "space",
                score = 15,
                reason = "Flexible modular configuration"
            }
        },

        highlights = new[]
        {
            "Seats up to 6 people",
            "Modular configuration",
            "Deep comfortable seating",
            "Modern neutral design"
        }
    },


    new
    {
        key = "luna",

        name = "Luna Designer Sofa",

        imageUrl =
            "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

        url =
            "https://example.com/products/luna-designer-sofa",

        shortDescription =
            "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

        description =
            "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

        ctaText = "View Luna Sofa",

        /*
         * DecisionOS scoring rules
         */
        rules = new[]
        {
            new
            {
                questionKey = "users",
                answerValue = "small",
                score = 25,
                reason = "Ideal for smaller spaces"
            },

            new
            {
                questionKey = "users",
                answerValue = "medium",
                score = 20,
                reason = "Works well for everyday living"
            },

            new
            {
                questionKey = "users",
                answerValue = "large",
                score = 10,
                reason = "Can accommodate larger households"
            },

            new
            {
                questionKey = "priority",
                answerValue = "style",
                score = 50,
                reason = "Elegant contemporary design"
            },

            new
            {
                questionKey = "priority",
                answerValue = "space",
                score = 40,
                reason = "Space-efficient footprint"
            },

            new
            {
                questionKey = "priority",
                answerValue = "comfort",
                score = 25,
                reason = "Comfortable premium seating"
            }
        },

        highlights = new[]
        {
            "Contemporary design",
            "Space-efficient",
            "Premium upholstery",
            "Multiple color options"
        }
    }
}
            //////////recommendations = new[]
            //////////{
            //////////    new
            //////////    {
            //////////        key = "oslo",

            //////////        name = "Oslo Modular Sofa",

            //////////        matchScore = 92,

            //////////        imageUrl =
            //////////            "https://images.unsplash.com/photo-1555041469-a586c61ea9bc",

            //////////        url =
            //////////            "https://example.com/products/oslo-modular-sofa",

            //////////        shortDescription =
            //////////            "A spacious, comfortable modular sofa designed for modern family living.",

            //////////        description =
            //////////            "The Oslo Modular Sofa gives you flexible seating with a strong focus on comfort and everyday practicality.",

            //////////        reasons = new[]
            //////////        {
            //////////            "Great for larger groups",

            //////////            "Comfort-focused design",

            //////////            "Flexible modular configuration",

            //////////            "Ideal for everyday family use"
            //////////        },

            //////////        highlights = new[]
            //////////        {
            //////////            "Seats up to 6 people",

            //////////            "Modular configuration",

            //////////            "Deep comfortable seating",

            //////////            "Modern neutral design"
            //////////        },

            //////////        ctaText = "View Oslo Sofa"
            //////////    },

            //////////    new
            //////////    {
            //////////        key = "luna",

            //////////        name = "Luna Designer Sofa",

            //////////        matchScore = 94,

            //////////        imageUrl =
            //////////            "https://images.unsplash.com/photo-1550226891-ef816aed4a98",

            //////////        url =
            //////////            "https://example.com/products/luna-designer-sofa",

            //////////        shortDescription =
            //////////            "A stylish designer sofa for customers who want modern aesthetics without sacrificing comfort.",

            //////////        description =
            //////////            "The Luna Designer Sofa combines contemporary styling with a compact footprint, making it a strong choice for design-conscious spaces.",

            //////////        reasons = new[]
            //////////        {
            //////////            "Elegant contemporary design",

            //////////            "Space-efficient footprint",

            //////////            "Ideal for style-focused buyers",

            //////////            "Perfect for modern interiors"
            //////////        },

            //////////        highlights = new[]
            //////////        {
            //////////            "Contemporary design",

            //////////            "Space-efficient",

            //////////            "Premium upholstery",

            //////////            "Multiple color options"
            //////////        },

            //////////        ctaText = "View Luna Sofa"
            //////////    }
            //////////}
        };

        return Ok(configuration);


        ///////////////////////////////////////////////
        ///
        /// 
        //////////if (publicKey == "pk_demo_sofa_finder")
        //////////{
        //////////    return Ok(new
        //////////    {
        //////////        title = "Find Your Perfect Sofa",

        //////////        questions = new[]
        //////////        {
        //////////        new
        //////////        {
        //////////            key = "users",
        //////////            text = "How many people will use the sofa?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "1–2 people", value = "small" },
        //////////                new { text = "3–4 people", value = "medium" },
        //////////                new { text = "5+ people", value = "large" }
        //////////            }
        //////////        },
        //////////        new
        //////////        {
        //////////            key = "priority",
        //////////            text = "What matters most?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "Comfort", value = "comfort" },
        //////////                new { text = "Style", value = "style" },
        //////////                new { text = "Space saving", value = "space" }
        //////////            }
        //////////        }
        //////////    },

        //////////        recommendations = new[]
        //////////        {
        //////////        new { key = "oslo", name = "Oslo Modular Sofa" },
        //////////        new { key = "luna", name = "Luna Designer Sofa" }
        //////////    }
        //////////    });
        //////////}

        //////////return NotFound();

        ///////////////////////////////////////////////////////////////////////

        //////////if (publicKey != "pk_demo_sofa_finder")
        //////////    return NotFound();

        //////////var configuration = new
        //////////{
        //////////    title = "Find Your Perfect Sofa",

        //////////    questions = new[]
        //////////    {
        //////////        new
        //////////        {
        //////////            key = "users",
        //////////            text = "How many people will use the sofa?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "1–2 people", value = "small" },
        //////////                new { text = "3–4 people", value = "medium" },
        //////////                new { text = "5+ people", value = "large" }
        //////////            }
        //////////        },

        //////////        new
        //////////        {
        //////////            key = "priority",
        //////////            text = "What matters most to you?",
        //////////            options = new[]
        //////////            {
        //////////                new { text = "Comfort", value = "comfort" },
        //////////                new { text = "Style", value = "style" },
        //////////                new { text = "Space saving", value = "space" }
        //////////            }
        //////////        }
        //////////    },

        //////////    recommendations = new[]
        //////////    {
        //////////        new
        //////////        {
        //////////            key = "oslo",
        //////////            name = "Oslo Modular Sofa"
        //////////        },

        //////////        new
        //////////        {
        //////////            key = "luna",
        //////////            name = "Luna Designer Sofa"
        //////////        }
        //////////    }
        //////////};

        //////////return Ok(configuration);

        //////////////////////////////////////////////

        ////var widget = await db.Widgets.AsNoTracking()
        ////    .FirstOrDefaultAsync(x => x.PublicKey == publicKey && x.Status == "Published");

        ////if (widget is null) return NotFound();

        ////var version = await db.WidgetVersions.AsNoTracking()
        ////    .FirstOrDefaultAsync(x => x.WidgetId == widget.Id &&
        ////                              x.VersionNumber == widget.PublishedVersion &&
        ////                              x.Status == "Published");

        ////if (version is null) return NotFound();

        ////return Content(version.ConfigurationJson, "application/json");

        ////return Ok();

    }






}
