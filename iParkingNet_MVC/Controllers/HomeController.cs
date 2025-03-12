using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iParkingNet_MVC
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 404;

            return RedirectToAction("404", "Error");
        }
    }
}
