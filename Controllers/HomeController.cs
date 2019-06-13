using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YobitTradingBot.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Bot()
        {
            if (!User.Identity.IsAuthenticated)
            {
               return RedirectToAction("Register", "Account");
            }

            ViewBag.Message = "Bot page.";

            return View();
        }

    }
}