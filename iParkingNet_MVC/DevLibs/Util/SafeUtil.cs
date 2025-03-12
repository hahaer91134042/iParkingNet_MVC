using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// SafeUtil 的摘要描述
/// </summary>
public class SafeUtil
{
    public static void AddNoOpenerAttribute(WebControl control)
    {
        control.Attributes.Add("rel", "noopener");
    }
    public static void AddNoOpenerAttribute(HtmlControl control)
    {
        control.Attributes.Add("rel", "noopener");
    }
}