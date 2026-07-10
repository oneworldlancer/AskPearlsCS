using Microsoft.AspNetCore.Mvc;

namespace AskPearlsCS_PORTAL.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
