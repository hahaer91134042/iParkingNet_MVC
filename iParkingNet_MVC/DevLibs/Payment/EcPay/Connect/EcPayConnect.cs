using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// EcPayConnect 的摘要描述
/// </summary>
public class EcPayConnect:IDisposable
{
    private HttpContext context = HttpContext.Current;
    private Dictionary<string, string> valuePairs = new Dictionary<string, string>();
    private string formName = "ecpayForm";
    private string method = "post";
    private string url = "";
    
    public void send(EcPayRequest request)
    {
        url = request.url();
        request.CheckMacValue = request.getCheckCode();

        var properties = request.filterEcPayProperty(true);

        properties.ForEach(p =>
        {
            valuePairs.Add(p.Name, p.GetValue(request, null).ToString());
        });

        postForm();
    }

    private void postForm()
    {
        context.Response.Clear();
        context.Response.Write("<html><head></head>");
        context.Response.Write(string.Format("<body onload=\"document.{0}.submit()\">",formName));
        context.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >",formName, method,url));

        valuePairs.Foreach((key, value) =>
        {
            context.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", HttpUtility.HtmlEncode(key), HttpUtility.HtmlEncode(value)));
        });

        context.Response.Write("</form>");
        context.Response.Write("</body></html>");
        context.Response.End();

    }


    public void Dispose()
    {
        context = null;
        valuePairs.Clear();
    }
}