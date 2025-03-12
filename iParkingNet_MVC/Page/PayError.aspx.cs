using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iParkingNet_MVC.Page
{
    public partial class PayError : BaseWebPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            NoLabel.Text = orderNo;

        }

        private string orderNo = "";

        protected override void GetData(SqlContext sqlHelper)
        {
            orderNo = Request["no"];
            if (orderNo.isNullOrEmpty())
                Response.Redirect("~/Error/404");
        }
    }
}