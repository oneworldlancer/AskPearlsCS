using AskPearlsCS_PORTAL.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace AskPearlsCS_PORTAL.Controllers
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
            "Index_12"//,
            //"Index_13",
            //"Index_14",
            //"Index_15"
        };

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {

            var index = Random.Shared.Next(DesignViews.Length);
          
        return View(DesignViews[index]);
            //    return View("Index_7");

            //var index = (int)((uint)Interlocked.Increment(ref _nextDesign) % (uint)DesignViews.Length);
            //return View(DesignViews[index]);
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
