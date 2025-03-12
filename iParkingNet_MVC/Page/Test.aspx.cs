using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevLibs;

namespace iParkingNet_MVC.Page
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ord = Request["ord"];
            Ord.InnerText = ord;
            var str = $"file dir->{WebUtil.getFileDir()}  web url->{WebUtil.getWebURL()}  ip->{WebUtil.GetUserIP()}";
            Log.d($"test str->{str}");
            Msg.InnerText = str;

            var member = new Member().Also(m => m.CreatById(18));
            Log.d($"test member->{member.toJsonString()}");

        
        }
    }
}