using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// MailMsg 的摘要描述
/// </summary>
public class MailMsg
{
    public static MailMsg New()
    {
        return new MailMsg();
    }
    public string title = "";
    public StringBuilder body = new StringBuilder();
    public bool isTemplate = false;

    public MailMsg setTitle(string title)
    {
        this.title = title;
        return this;
    }
    public MailMsg useHtmlTemplate(bool isUse)
    {
        isTemplate = isUse;
        return this;
    }
    public MailMsg AppendRawMsg(string msg)
    {
        body.Append(msg);
        return this;
    }
    public MailMsg AppendMsg(string msg)
    {
        body.AppendLine("<font color=\"black\">" + TextUtil.formatSpaceToHtml(msg) + "</font><br/>");
        return this;
    }
    public MailMsg AppendUrl(string text, string url)
    {
        AppendMsg(new MsgBundle()
        {
            Text = text,
            Url = url
        });
        return this;
    }
    public MailMsg AppendMsg(MsgBundle bundle)
    {
        body.AppendLine(bundle.getSpan() + "<br/>");
        //ResponseBuilder.getInstance().Append(getMsg()).print().end();
        return this;
    }
    public void copy(MailMsg msg)
    {
        title = msg.title;
        body = msg.body;
        isTemplate = msg.isTemplate;
    }
    public string getMsg()
    {
        if (isTemplate)
            return string.Format(getTempelet(), body.ToString());
        else
            return body.ToString();
    }
    private string getTempelet()
    {
        String temp = "<div>";
        temp += "<p>{0}</p>";
        //temp += "<p>URL：<a href=\"" + vsWebURL.ToString() + "/tc/verify.ashx?vc=" + psVerifyCode + "&id=" + psID.ToString() + "&chk=" + psSecureChk + "\">驗証郵件</a></p>\r\n";
        temp += "</div>";
        return temp;
    }
}