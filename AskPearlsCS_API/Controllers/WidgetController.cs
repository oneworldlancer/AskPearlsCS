//using AskPearls.Infrastructure;
using AskPearlsCS_BLL.app_Service;
using AskPearlsCS_BLL.DecisionOSManager;

using Microsoft.AspNetCore.Mvc;
using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AskPearlsCS_API.Controllers;

public sealed record WidgetDesignDraftRequest(
    [property: JsonPropertyName("primary")] string Primary,
    [property: JsonPropertyName("background")] string Background,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("font")] string Font,
    [property: JsonPropertyName("fontSize")] int FontSize,
    [property: JsonPropertyName("radius")] int Radius,
    [property: JsonPropertyName("shadow")] bool Shadow,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("position")] string Position,
    [property: JsonPropertyName("buttonShape")] string ButtonShape,
    [property: JsonPropertyName("heading")] string Heading,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("buttonText")] string ButtonText,
    [property: JsonPropertyName("footer")] string Footer,
    [property: JsonPropertyName("theme")] string Theme = "custom");

[ApiController]
[Route("api/widgets")]
public class WidgetController : ControllerBase
//public class WidgetController(DBContextModel db) : ControllerBase
{

    private readonly IWidget_Service _widgetService;
    private readonly IWidgetVersion_Service _widgetVersionService;
    private readonly IQuestion_Service _questionService;
    private readonly DBContextModel _db;
    private readonly IWebHostEnvironment _environment;

    public WidgetController(
       IWidget_Service widgetService,
       IWidgetVersion_Service widgetVersionService,
       IQuestion_Service questionService,
       DBContextModel db,
       IWebHostEnvironment environment)
    {
        _widgetService = widgetService;
        _widgetVersionService = widgetVersionService;
        _questionService = questionService;
        _db = db;
        _environment = environment;
    }

    // Local demo identity from the user's test data. These routes are disabled
    // outside Development and must be replaced by the portal's Google login model.
    private const string DemoOwnerUserTokenId = "1101";

    [HttpPut("{publicKey}/draft/configuration")]
    public async Task<IActionResult> SaveDesignDraft(
        string publicKey,
        [FromBody] WidgetDesignDraftRequest? request)
    {
        if (!TryGetDemoOwner(out var error)) return error!;
        if (string.IsNullOrWhiteSpace(publicKey) || !IsValidDesign(request))
            return BadRequest(new { success = false, message = "Widget key or design settings are invalid." });

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var versions = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId && x.WidgetPublicKey == publicKey)
            .ToListAsync();
        var drafts = versions.Where(x => x.WidgetStatusCode == "draft" &&
                                         x.IsActive == true && x.IsVisible == true).ToList();
        if (drafts.Count > 1)
            return Conflict(new { success = false, message = "More than one active draft exists for this widget." });

        var json = JsonSerializer.Serialize(request);
        var draft = drafts.SingleOrDefault();
        if (draft is null)
        {
            var publishedVersions = versions.Where(x => x.WidgetStatusCode == "published" &&
                                                        x.IsActive == true && x.IsVisible == true).ToList();
            if (publishedVersions.Count != 1 || string.IsNullOrWhiteSpace(publishedVersions[0].WidgetVersionTokenId))
                return Conflict(new { success = false, message = "A single published content version is required before saving a design draft." });

            var publishedNumber = int.TryParse(widget.WidgetPublishedVersion, out var n) ? n : 0;
            var maxNumber = versions.Select(x => int.TryParse(x.WidgetVersionNumber, out var v) ? v : 0)
                                     .DefaultIfEmpty(0).Max();
            var number = Math.Max(publishedNumber, maxNumber) + 1;
            draft = new AskpearlsPortalUserWebsiteWidgetVersionList
            {
                AccountType = widget.AccountType,
                OwnerUserTokenId = widget.OwnerUserTokenId,
                OwnerMobileNumberTokenId = widget.OwnerMobileNumberTokenId,
                OrganizationTokenId = widget.OrganizationTokenId,
                CompanyTokenId = widget.CompanyTokenId,
                CompanyName = widget.CompanyName,
                WidgetTokenId = widget.WidgetTokenId,
                WebsiteTokenId = widget.WebsiteTokenId,
                WebsiteName = widget.WebsiteName,
                WidgetPublicKey = widget.WidgetPublicKey,
                // Design-only drafts reuse the published content token so the
                // existing questions, answers, rules and recommendations remain linked.
                WidgetVersionTokenId = publishedVersions[0].WidgetVersionTokenId,
                WidgetVersionNumber = number.ToString(),
                WidgetConfigurationJson = json,
                WidgetStatusCode = "draft",
                WidgetStatusText = "draft",
                WidgetStatus = "draft",
                WidgetPublishedVersion = widget.WidgetPublishedVersion,
                ActiveStatus = "pending",
                IsNew = true,
                IsActive = true,
                IsVisible = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _db.AskpearlsPortalUserWebsiteWidgetVersionLists.Add(draft);
        }
        else
        {
            draft.WidgetConfigurationJson = json;
            draft.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return Ok(new { success = true, status = "draft", versionNumber = draft.WidgetVersionNumber });
    }

    [HttpPost("{publicKey}/publish")]
    public async Task<IActionResult> PublishDesignDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var drafts = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "draft" &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        if (drafts.Count != 1)
            return Conflict(new
            {
                success = false,
                message = drafts.Count == 0
                ? "Save a draft before publishing." : "More than one active draft exists."
            });

        var draft = drafts[0];
        if (!IsValidStoredDesign(draft.WidgetConfigurationJson))
            return Conflict(new { success = false, message = "Draft configuration is missing or invalid." });

        await using var transaction = await _db.Database.BeginTransactionAsync();
        var published = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "published" &&
                        x.IsActive == true && x.IsVisible == true)
            .ToListAsync();
        foreach (var previous in published)
        {
            previous.WidgetStatusCode = "archived";
            previous.WidgetStatusText = "archived";
            previous.WidgetStatus = "archived";
            previous.UpdatedAtUtc = DateTime.UtcNow;
        }

        draft.WidgetStatusCode = "published";
        draft.WidgetStatusText = "published";
        draft.WidgetStatus = "published";
        draft.WidgetPublishedVersion = draft.WidgetVersionNumber;
        draft.IsNew = false;
        draft.UpdatedAtUtc = DateTime.UtcNow;
        widget.WidgetPublishedVersion = draft.WidgetVersionNumber;
        widget.WidgetStatusCode = "published";
        widget.WidgetStatusText = "published";
        widget.WidgetStatus = "published";

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return Ok(new { success = true, status = "published", versionNumber = draft.WidgetVersionNumber });
    }

    [HttpGet("{publicKey}/draft/configuration")]
    public async Task<IActionResult> GetDesignDraft(string publicKey)
    {
        if (!TryGetDemoOwner(out var error)) return error!;

        var widget = await _db.AskpearlsPortalUserWebsiteWidgetLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WidgetPublicKey == publicKey &&
                                      x.OwnerUserTokenId == DemoOwnerUserTokenId &&
                                      x.IsActive == true && x.IsVisible == true);
        if (widget is null)
            return NotFound(new { success = false, message = "Widget was not found for the demo owner." });

        var draft = await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .Where(x => x.WidgetTokenId == widget.WidgetTokenId &&
                        x.WidgetPublicKey == publicKey && x.WidgetStatusCode == "draft" &&
                        x.IsActive == true && x.IsVisible == true)
            .OrderByDescending(x => x.UwebsiteWidgetVersionId)
            .FirstOrDefaultAsync();

        var version = draft ?? await _db.AskpearlsPortalUserWebsiteWidgetVersionLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WidgetTokenId == widget.WidgetTokenId &&
                                      x.WidgetPublicKey == publicKey &&
                                      x.WidgetVersionNumber == widget.WidgetPublishedVersion &&
                                      x.WidgetStatusCode == "published" &&
                                      x.IsActive == true && x.IsVisible == true);

        if (string.IsNullOrWhiteSpace(version?.WidgetConfigurationJson))
            return Ok(new { success = true, status = draft is null ? "published" : "draft", configuration = (object?)null });

        try
        {
            using var document = JsonDocument.Parse(version.WidgetConfigurationJson);
            return Ok(new
            {
                success = true,
                status = version.WidgetStatusCode,
                versionNumber = version.WidgetVersionNumber,
                configuration = document.RootElement.Clone()
            });
        }
        catch (JsonException)
        {
            return Conflict(new { success = false, message = "The saved widget design JSON is invalid." });
        }
    }

    private bool TryGetDemoOwner(out IActionResult? error)
    {
        error = null;
        if (!_environment.IsDevelopment())
        {
            error = NotFound();
            return false;
        }
        if (Request.Headers["X-Demo-Owner-User-Token"].ToString() != DemoOwnerUserTokenId)
        {
            error = Unauthorized(new { success = false, message = "Demo owner login is required." });
            return false;
        }
        return true;
    }

    private static bool IsValidDesign(WidgetDesignDraftRequest? value)
    {
        if (value is null || !IsHex(value.Primary) || !IsHex(value.Background) || !IsHex(value.Text) ||
            value.FontSize is < 12 or > 18 || value.Radius is < 0 or > 28 || value.Width is < 300 or > 520 ||
            value.Position is not ("bottom-right" or "bottom-left" or "center") ||
            value.ButtonShape is not ("rounded" or "pill" or "square") ||
            value.Theme is not ("custom" or "light" or "dark") ||
            value.Heading is null || value.Description is null || value.ButtonText is null || value.Footer is null)
            return false;

        var fonts = new[] { "Inter, Arial, sans-serif", "Arial, sans-serif", "Georgia, serif", "Verdana, sans-serif" };
        return fonts.Contains(value.Font, StringComparer.Ordinal) && value.Heading.Length <= 100 &&
               value.Description.Length <= 250 && value.ButtonText.Length <= 40 && value.Footer.Length <= 80;
    }

    private static bool IsValidStoredDesign(string? json)
    {
        try
        {
            return IsValidDesign(JsonSerializer.Deserialize<WidgetDesignDraftRequest>(
                json ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }
        catch (JsonException) { return false; }
    }

    private static bool IsHex(string? value) =>
        Regex.IsMatch(value ?? "", "^#[0-9a-fA-F]{6}$");



    [HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var widget = await _widgetService
            .GetPublishedWidgetAsync(publicKey);

        if (widget == null)
        {
            return NotFound(new
            {
                message = "Published widget was not found."
            });
        }

        return Ok(widget);
    }




    [HttpGet("{publicKey}/version")]
    public async Task<IActionResult> GetPublishedVersion(
    string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var version = await _widgetVersionService
            .GetPublishedVersionAsync(publicKey);

        if (version == null)
        {
            return NotFound(new
            {
                message = "Published widget version was not found."
            });
        }

        return Ok(new
        {
            version.VersionTokenId,
            version.VersionNumber,
            version.Status,
            version.ConfigurationJson
        });
    }





    [HttpGet("{publicKey}/questions")]
    public async Task<IActionResult> GetPublishedQuestions(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest(new
            {
                message = "Widget public key is required."
            });
        }

        var questions =
            await _questionService.GetPublishedQuestionsAsync(publicKey);

        if (questions.Count == 0)
        {
            return NotFound(new
            {
                message = "No published questions were found for this widget."
            });
        }

        return Ok(new
        {
            widgetPublicKey = publicKey,
            questions
        });
    }







    //[HttpGet("{publicKey}")]
    public async Task<IActionResult> GetPublished_X3(string publicKey)
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
