using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iParkingNet_MVC.Page
{
    public partial class EcPayBack : BaseWebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var data = Request.Form;
            var response = data.toEcPayObj<EcPayResponse>();

            rBuilder.Append("back->", data);
            rBuilder.Append("obj->", response);
            rBuilder.Append("check->", response.checkEcPay());

            var propertys = response.filterEcPayProperty(true);
            propertys.ForEach(p =>
            {
                rBuilder.Append(p.Name, p.GetValue(response).ToString());
            });

            rBuilder.print();
        }
    }
}