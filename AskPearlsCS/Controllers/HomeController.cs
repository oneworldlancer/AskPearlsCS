using AskPearlsCS.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace AskPearlsCS.Controllers
{
    public class HomeController : Controller
    {


        private static int _nextDesign = -1;
        private static readonly string[] DesignViews =
     {
            "Index_1",
            "Index_2",
            "Index_3",
            "Index_4",
            "Index_5",
            "Index_6",
            "Index_7",
            "Index_8"  ,
            "Index_9",
            "Index_10",
            "Index_11",
            "Index_12",
            "Index_13",
            "Index_14",
            "Index_15"
        };


        private static readonly string[] HeaderSlogans =
      {
            "Capture. Engage. Convert.",
            "Connect. Discover. Grow.",
            "Imagine. Build. Succeed.",
            "Explore. Decide. Achieve.",
            "Discover. Connect. Thrive.",
            "Create. Collaborate. Innovate.",
            "Think. Plan. Execute.",
            "Inspire. Empower. Transform.",
            "Capture. Understand. Deliver.",
            "Simplify. Connect. Succeed.",
            "Explore. Learn. Grow.",
            "Connect. Analyze. Decide.",
            "Engage. Inspire. Convert.",
            "Discover. Decide. Deliver.",
            "Listen. Understand. Act.",
            "Collect. Analyze. Improve.",
            "Imagine. Connect. Create.",
            "Understand. Decide. Grow.",
            "Capture. Connect. Succeed.",
            "Explore. Innovate. Transform.",
            "Connect. Empower. Achieve.",
            "Discover. Engage. Thrive.",
            "Think. Create. Grow.",
            "Simplify. Empower. Deliver.",
            "Learn. Adapt. Succeed.",
            "Create. Connect. Convert.",
            "Discover. Design. Deliver.",
            "Imagine. Explore. Achieve.",
            "Analyze. Optimize. Grow.",
            "Connect. Collaborate. Succeed."
        };


        private static string GetRandomHeaderSlogan()
        {
            return HeaderSlogans[
                Random.Shared.Next(HeaderSlogans.Length)
            ];
        }

        public IActionResult Index()
        {

            // Random homepage design (1–15)
            int designIndex = Random.Shared.Next(DesignViews.Length);



            ViewBag.HeaderSlogan = GetRandomHeaderSlogan();


          //  return View(DesignViews[designIndex]);

              return View("Index_1");


            //return View();
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
