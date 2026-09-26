//using AskPearls.Infrastructure;
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
