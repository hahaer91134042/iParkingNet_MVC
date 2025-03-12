using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevLibs;
using Eki_NewebPay;

namespace iParkingNet_MVC.Page
{
    public partial class Return : BaseWebPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //CancelBtn.Attributes.Add("href", EkiScheme.CheckoutFinish.Uri().ToString());
            if (returnModel.Status == "SUCCESS")
            {
                var useToken = returnModel.MPG.Result.TokenUseStatus > 0;

                var url = !useToken ? EkiScheme.CheckoutFinish.Uri(
                        order.SerialNumber,
                        returnModel.MPG.Result.Amt,
                        returnModel.MPG.Result.Card4No
                        ).ToString() : EkiScheme.CheckoutFinish.Uri(
                        order.SerialNumber,
                        returnModel.MPG.Result.Amt,
                        returnModel.MPG.Result.Card4No,
                        returnModel.MPG.Result.TokenLife
                        ).ToString();

                Response.Redirect(url);

            }
            else
            {
                Response.Redirect("~/page/payerror.aspx?no=" + order.SerialNumber);
            }
        }

        private EkiOrder order;
        private NewebPayMPGReturn returnModel;
        protected override void GetData(SqlContext sqlHelper)
        {
            try
            {
                var ord = Request["ord"];
                //if (!string.IsNullOrEmpty(Request["lan"]))
                //    Lan = Request["lan"].toEnum<LanguageFamily>();
                order = (from o in GetTable<EkiOrder>()
                         where o.beEnable
                         where o.UniqueID.equal(ord)
                         select o).FirstOrDefault();
                if (order.isNullOrEmpty())
                    throw new ArgumentNullException();

                returnModel = NewebPayMPGReturn.Load(Request);

                //rBuilder.Append($"OrdUnique=>{ord}");
                //rBuilder.Append("Order Id->" + order.Id);
                //rBuilder.Append("Status->" + returnModel.Status)
                //    .Append("ID->" + returnModel.MerchantID)
                //    .Append("Info->" + returnModel.TradeInfo)
                //    .Append("sha->" + returnModel.TradeSha);


                if (returnModel.Parse())
                {
                    //rBuilder.PrintContent(returnModel);

                    //Log.print($"Return page model->{returnModel.toJsonString()}");

                    if (returnModel.Status == "SUCCESS")
                    {
                        PayStatusText.Text = "交易成功";
                    }
                    else
                    {
                        PayStatusText.Text = "交易失敗";
                        ErrorMsg.Text = returnModel.MPG.Message;
                    }

                }
                else
                {
                    //rBuilder.Append("Can`t parse model");
                    PayStatusText.Text = "交易結果出現異常";
                }

                //rBuilder.print();
            }
            catch (Exception)
            {
                Response.Redirect("~/Error/404");
            }
        }
    }
}