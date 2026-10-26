using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public IActionResult LoadSection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                return BadRequest("Section is required.");
            }

            switch (section.Trim().ToLowerInvariant())
            {
                case "dashboard":
                    return PartialView("~/Views/Dashboard/_DashboardContent.cshtml");

                case "widgets":
                    return PartialView("~/Views/Dashboard/_MyWidgetsContent.cshtml");

                case "widget-builder":
                    return PartialView("_WidgetBuilderContent");

                case "questions":
                    return PartialView("_QuestionsAnswersContent");

                case "recommendation-items":
                    return PartialView("_RecommendationItemsContent");

                case "decision-rules":
                    return PartialView("_DecisionRulesContent");
                //return PartialView("~/Views/Dashboard/_DecisionRulesContent.cshtml");

                case "design-customize":
                    return PartialView("_DesignCustomizeContent");


                case "publish-embed":
                    return PartialView("_PublishEmbedContent");

                case "analytics":
                    return PartialView("_AnalyticsContent");

                case "account-settings":
                    return PartialView("_AccountSettingsContent");

                case "scoring-preview":
                    return PartialView("~/Views/Dashboard/_ScoringPreviewContent.cshtml");


                default:
                    return NotFound("Portal section not found.");
            }
        }




        [HttpGet]
        public IActionResult LoadSection_X1(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                return BadRequest("Section is required.");

            switch (section.ToLowerInvariant())
            {
                case "dashboard":
                    return PartialView("_DashboardContent");

                default:
                    return NotFound(
                        $"Section '{section}' was not found.");
            }
        }
    }
}
