using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iParkingNet_MVC
{
    [RoutePrefix("Error")]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("Error");
        }

        [Route("{code:int:regex(\\d{3})}")]
        [Route("Status/{code:int:regex(\\d{3})}")]
        public ActionResult Status(int code)
        {

            Response.StatusCode = code;
            var status = code.toEnum<HttpStatusCode>();
            var msg = "該頁面不存在";
            switch (status)
            {
                case HttpStatusCode.Unauthorized:
                    msg = "使用權限不足!!";
                    break;
                case HttpStatusCode.Forbidden:
                    msg = "拒絕連線";
                    break;
                default:
                    msg = "該頁面不存在";
                    break;
            }
            ViewBag.msg = msg;
            return View();
        }
    }
}