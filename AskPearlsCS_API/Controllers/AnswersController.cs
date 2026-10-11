using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_API.Controllers
{
    public class AnswersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
