using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using Eki_NewebPay;

/// <summary>
/// 將訂單完成Checkout的流程
/// </summary>
public class OrderCheckOutProcess:BaseProcess
{
    private EkiOrder order;
    public Func<bool> checkSuccess=()=>false;//必填
    public Func<string> tradeNo = () => "";//選填
    public Func<string> card4No = () => "";//必填
    public OrderCheckOutProcess(EkiOrder o)
    {
        order = o;
    }

    public override void run()
    {
        if (checkSuccess())
        {
            switch (order.StatusEnum)
            {
                case OrderStatus.Cancel:
                    var cancel = new OrderCancel().Also(c => c.CreatById(order.Id));
                    cancel.Paid = true;
                    cancel.Update();
                    break;
                default:
                    order.StatusEnum = OrderStatus.CheckOut;
                    //模擬用
                    //OrderPayRecord.creat(order.Id).Also(record => record.Insert());
                    break;
            }

            sendPaymentSuccessBroadcast(order);
            startInvoice(order);
        }
        else
        {
            order.StatusEnum = OrderStatus.PayError;
        }
        order.Update();
    }

    private void sendPaymentSuccessBroadcast(EkiOrder order)
    {
        try
        {
            //var loc = (from l in EkiSql.ppyp.table<Location>()
            //           where order.LocationId == l.Id
            //           select l).First();
            var loc = order.Location;
            var manager = loc.Member;
            var checkout = order.CheckOut;
            //var checkout = (from c in EkiSql.ppyp.table<EkiCheckOut>()
            //                where c.OrderId == order.Id
            //                select c).First();

            var orderNormalCost = OrderManager
                .OrderCalculater
                .calOrderCost(checkout.Date.standarCheckOutTime(order.ReservaTime) - order.ReservaTime.getStartTime(), order).toCurrency(order);

            new ManagerGetPayContent
            {
                OrderSerNum = order.SerialNumber,
                Cost = orderNormalCost
            }.sendTo(manager);
        }
        catch (Exception e)
        {
            Log.e("OrderCheckOutProcess sendBroadcast error", e);
        }
    }

    private void startInvoice(EkiOrder order)
    {
        try
        {
            var invoice = order.Invoice;

            var member = order.Member;

            var taxAmt = (order.Cost.toDouble() * (PayConfig.Invoice.TaxRate / 100.0)).Round().toInt();
            var amt = order.Cost.toInt() - taxAmt;
            var request = invoice != null ? new NewebPayInvoiceModel
            {
                TransNum = tradeNo(),
                MerchantOrderNo = invoice.SerNO,
                Category = invoice.BuyerUBN.isNullOrEmpty() ? NewebPayInvoice.Category_B2C : NewebPayInvoice.Category_B2B,
                BuyerName = invoice.Name.isNullOrEmpty() ? member.PhoneNum : invoice.Name,
                BuyerUBN = invoice.BuyerUBN.isNullOrEmpty() ? "" : invoice.BuyerUBN,
                BuyerAddress = invoice.Address,
                BuyerEmail = invoice.Email.isNullOrEmpty() ? member.Mail : invoice.Email,
                CarrierType = invoice.payCarrierType(),
                CarrierNum = invoice.payCarrierType().isNullOrEmpty() ? "" : invoice.CarrierNum,
                LoveCode = invoice.payCarrierType().isNullOrEmpty() ? invoice.BuyerUBN.isNullOrEmpty() ? invoice.LoveCode : "" : "",
                PrintFlag = invoice.BuyerUBN.isNullOrEmpty() ? invoice.payCarrierType().isNullOrEmpty() && invoice.LoveCode.isNullOrEmpty() ? NewebPayInvoice.PrintFlag_Y : NewebPayInvoice.PrintFlag_N : NewebPayInvoice.PrintFlag_Y,
                ItemName = PayConfig.Invoice.ItemName,
                ItemCount = 1,
                ItemUnit = PayConfig.Invoice.ItemUnit,
                TaxRate = PayConfig.Invoice.TaxRate
            } : new NewebPayInvoiceModel
            {
                TransNum = tradeNo(),
                MerchantOrderNo = order.SerialNumber,
                Category = NewebPayInvoice.Category_B2C,
                BuyerName = member.PhoneNum,
                BuyerUBN = "",
                BuyerAddress = "",
                BuyerEmail = member.Mail,
                CarrierType = "",
                CarrierNum = "",
                LoveCode = PayConfig.Invoice.LoveCode,
                ItemName = PayConfig.Invoice.ItemName,
                ItemCount = 1,
                ItemUnit = PayConfig.Invoice.ItemUnit,
                TaxRate = PayConfig.Invoice.TaxRate
            };

            request.Amt = amt;
            request.TaxAmt = taxAmt;

            request.TotalAmt = order.Cost.toInt();
            request.ItemAmt = invoice.BuyerUBN.isNullOrEmpty() ? order.Cost.toInt() : amt;

            //request.ItemPrice = invoice.BuyerUBN.isNullOrEmpty() ? order.Cost.toInt() : amt;

            request.Comment = $"信用卡末四碼:{card4No()}";

            var response = NewebPay.Invo().Post(request);

            //Log.print($"Invoice response->{response.toJsonString()}");
            
            InvoiceReturn.Load(response).Also(result =>
            {
                result.MemberId = order.MemberId;
                result.OrderId = order.Id;
                result.InvoiceId = invoice.isNullOrEmpty() ? 0 : invoice.Id;
            }).Insert();
        }
        catch (Exception e)
        {
            Log.e("Checkout invoice error", e);
        }

    }
}