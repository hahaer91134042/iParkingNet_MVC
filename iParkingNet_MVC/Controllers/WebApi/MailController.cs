using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DevLibs;

[RoutePrefix("api/Mail")]
public class MailController : BaseApiController
{
    public class MailReq
    {
        public string from { get; set; }
        public string name { get; set; } = "";
        public bool isHtml { get; set; } = true;
        public List<MailContent> msg { get; set; } = new List<MailContent>();
    }

    public class MailContent
    {
        public string to { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }

    [HttpPost]
    [Route("Send")]
    public object Send(MailReq req)
    {
        try
        {
            //Log.print($"Mail send req->{req.toJsonString()}");

            var builder = MailConfig.creatBuilder();
            var smtp = builder.from(req.from)
                .setSenderName(req.name)
                .useHtmlBody(req.isHtml)
                .build();

            req.msg.ForEach(c =>
            {
                var msg = new MailMsg();
                msg.useHtmlTemplate(req.isHtml);
                msg.setTitle(c.title);
                msg.AppendRawMsg(c.content);
                //smtp.SendTo("hahaer91134042@eki.com.tw", msg);
                //Log.print($"mail msg->{msg.toJsonString()}");
                smtp.SendTo(c.to, msg);
            });


            

            smtp.Dispose();

            return new MailResp(true);
        }
        catch (Exception)
        {

        }
        return ResponseError();
    }

    public class MailResp : ResponseAbstractModel
    {
        public MailResp(bool successful) : base(successful)
        {
        }
    }

}
