using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

/// <summary>
/// BaseHtmlControl 的摘要描述
/// </summary>
public abstract class BaseHtmlControl: HtmlGenericControl
{
    public BaseHtmlControl(string tag) : base(tag)
    {

    }
}