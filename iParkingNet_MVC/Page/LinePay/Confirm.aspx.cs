using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Eki_LinePayApi_v3;

namespace iParkingNet_MVC.Page.LinePay
{
    public partial class Confirm : BaseWebPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            startConfirm();

        }

        private async void startConfirm()
        {
            var client = new Eki_LinePayApi_v3.LinePay.Client(EkiLinePay.Config);

            var confirm = client.ConfirmAsync(new LinePayConfirm
            {
                amount = order.Cost.toInt()
                //amount=1
            }, linePay.TransactionId);

            //confirm.Wait();

            var result = await confirm;

            linePay.ConfirmResult = result.toJsonString();
            linePay.Update();

            new OrderCheckOutProcess(order)
            {
                checkSuccess = () => result.returnCode == LineCode.Confirm.Success,
                card4No = () => result.returnCode == LineCode.Confirm.Success ? result.info.payInfo[0].card4No() : ""
            }.run();

            if (result.returnCode == LineCode.Confirm.Success)
            {
                var payInfo = result.info.payInfo[0];
                //Log.print($"payinfo->{payInfo.toJsonString()} card4No->{payInfo.card4No()}");

                var uri = EkiScheme.CheckoutFinish.Uri(
                order.SerialNumber,
                payInfo.amount,
                payInfo.card4No()
                ).ToString();

                //Log.print($"redirect->{uri}");

                Response.Redirect(uri);
            }
            else
            {
                Response.Redirect("~/page/payerror.aspx?no=" + order.SerialNumber);
            }

        }

        private EkiOrder order;
        private OrderLinePay linePay;

        protected override void GetData(SqlContext sqlHelper)
        {
            var orderSer = Request["ord"];
            var ekiOrderSer = Request["orderId"];
            var transactionId = Request["transactionId"];
            Log.d($"LinePay Confirm ord->{orderSer} ekiOrder->{ekiOrderSer} tid->{transactionId}");

            order = EkiSql.ppyp.data<EkiOrder>("where SerialNumber=@serial", new { serial = orderSer });

            //var record = EkiSql.ppyp.data<OrderPayRecord>(
            //    "where OrderId=@oid " +
            //    "and EkiOrderSerial=@eSerial"
            //    , new { oid = order.Id, eSerial = ekiOrderSer });

            //linePay = EkiSql.ppyp.data<OrderLinePay>(
            //    "where OrderSerial=@oSerial " +
            //    "and EkiSerial=@eSerial " +
            //    "and TransactionId=@tid"
            //    , new { oSerial = orderSer, eSerial = ekiOrderSer, tid = transactionId });

            linePay = (from r in EkiSql.ppyp.table<OrderPayRecord>()
                       join l in EkiSql.ppyp.table<OrderLinePay>() on r.Id equals l.RecordId
                       where r.OrderId == order.Id
                       where r.EkiOrderSerial == ekiOrderSer
                       where l.TransactionId.ToString() == transactionId
                       select l).FirstOrDefault();


            if (linePay == null)
            {
                Response.StatusCode = 404;
                Response.Redirect("~/Error/404");
            }
            else Response.StatusCode = 200;
        }
    }
}