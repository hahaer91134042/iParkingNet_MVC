using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MsgBundle 的摘要描述
/// </summary>
//<span style="border:2px red solid;font-size:12px;background-color:yellow;padding:10px;">
public class MsgBundle
{
    public int TextSize = 10;
    public ColorName TextColor = ColorName.Black;
    public string Text = "";
    private string url = "";//Avoid StackOverFlowExecption
    public string Url
    {
        get { return url; }
        set
        {
            url = value;
            isUrl = !string.IsNullOrEmpty(value);
        }
    }
    public bool isUrl = false;
    public string getSpan()
    {
        var spanFormat = "<font color={0} size=\"{1}\">{2}</font>";
        var urlFormat = "URL：<a href=\"{0}\">{1}</a>";
        if (isUrl)
            return string.Format(urlFormat, Url, Text);
        else
            return string.Format(spanFormat, TextColor.ToString(), TextSize, TextUtil.formatSpaceToHtml(Text));
    }
}