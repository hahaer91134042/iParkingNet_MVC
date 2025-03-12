using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iParkingNet_MVC.Handler
{
    /// <summary>
    /// EcPayReceiver 的摘要描述
    /// </summary>
    public class EcPayReceiver : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var response = context.Request.Form.toEcPayObj<EcPayResponse>();
            response.saveLog("EcPayBack");

            context.Response.Write("1");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}