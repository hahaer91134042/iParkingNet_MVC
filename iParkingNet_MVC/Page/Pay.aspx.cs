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
    public partial class Pay : BaseWebPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Log.d($"Pay page order->{order.SerialNumber}");
            try
            {
                switch (creditType)
                {
                    case PayCreditType.Agree:

                        //var mPayInfo = member.PayInfo;
                        //if (mPayInfo?.neweb?.useable()??false)
                        //{
                        //    postApiCredit(mPayInfo);
                        //}
                        //else
                        //{
                        //    postToMPG_CreditAgree();
                        //}

                        postToMPG_CreditAgree();
                        break;
                    default:
                        var creditModel = new NewebPayMPGModel.CreditCard()
                        {
                            TimeStamp = info.newebStamp,
                            LangType = member.memberLan == LanguageFamily.TC ? NewebPayMPG.Config.LangType_Tw : NewebPayMPG.Config.LangType_En,
                            MerchantOrderNo = orderNo,
                            Amt = info.amt.toInt(),//目前沒有貨幣類別的換算
                            ItemDesc = info.desc,
                            ReturnURL = order.getPayReturnUrl(),
                            NotifyURL = order.getPayNotifyUrl(),
                            TokenTerm = member.PhoneNum,
                            //ClientBackURL=WebUtil.getWebURL(),
                            Email = member.Mail
                        };

                        NewebPay.MPG().Post(creditModel);
                        break;
                }

            }
            catch (Exception ex)
            {
                //Log.e($"Pay page Load error", ex);
            }
        }

        //private void postApiCredit(MemberPayInfo mPayInfo)
        //{
        //    var creditModel = new NewebPayCreditModel
        //    {
        //        MerchantOrderNo = orderNo,
        //        Amt = info.amt.toInt(),//目前沒有貨幣類別的換算
        //        ProdDesc = info.desc,
        //        PayerEmail = order.Member.Mail,
        //        TokenTerm = order.Member.PhoneNum,
        //        TokenValue = mPayInfo.neweb.Token
        //    };

        //    var resp = NewebPay.CreditCard().Post(creditModel);

        //    Log.print($"Credit agree resp->{resp.toJsonString()}");

        //    NewebPayReturn.Load(resp).Also(rModel =>
        //    {
        //        rModel.OrderId = order.Id;
        //        rModel.Insert();

        //        new OrderCheckOutProcess(order)
        //        {
        //            checkSuccess = () => resp.Status == "SUCCESS",
        //            tradeNo = () => rModel.TradeNo,
        //            card4No = () => rModel.Card4No
        //        }.run();

        //        mPayInfo.neweb.TokenLife = resp.Result.TokenLife;
        //        mPayInfo.Update();

        //        var url = EkiScheme.CheckoutFinish.Uri(
        //                order.SerialNumber,
        //                rModel.Amt,
        //                rModel.Card4No,
        //                resp.Result.TokenLife
        //                ).ToString();

        //        Log.print($"To Uri->{url}");

        //        Response.Redirect(url, false);
        //        Context.ApplicationInstance.CompleteRequest();
        //    });
        //}

        private void postToMPG_CreditAgree()
        {
            var agreeModel = new NewebPayMPGModel.AgreeCredit()
            {
                TimeStamp = info.newebStamp,
                LangType = member.memberLan == LanguageFamily.TC ? NewebPayMPG.Config.LangType_Tw : NewebPayMPG.Config.LangType_En,
                MerchantOrderNo = orderNo,
                Amt = info.amt.toInt(),//目前沒有貨幣類別的換算
                ItemDesc = info.desc,
                EmailModify=1,
                ReturnURL = order.getPayReturnUrl(),
                NotifyURL = order.getPayNotifyUrl(),
                TokenTerm = member.PhoneNum,
                //ClientBackURL=WebUtil.getWebURL(),
                Email = member.Mail,
                OrderComment = "停車費約定信用卡"
            };

            NewebPay.MPG().Post(agreeModel);
        }


        //private LanguageFamily Lan = LanguageFamily.TC;
        private EkiOrder order;
        //private EkiCheckOut checkOut;
        //private OrderCancel cancel;
        private Member member;
        //private Location orderLocation;
        private string orderNo;
        private string creditType = "";
        private OrderManager.OrderPayInfo info;
        protected override void GetData(SqlContext sqlHelper)
        {
            try
            {
                //這樣做避免一些問題
                var ord = Request["ord"];
                //if (!string.IsNullOrEmpty(Request["lan"]))
                //    Lan = Request["lan"].toEnum<LanguageFamily>();
                creditType = Request.Headers[PayConfig.Flag.Eki_Pay_Credit];

                //Log.d($"Pay page PayType->{creditType}");

                order = (from o in GetTable<EkiOrder>()
                         where o.beEnable
                         where o.UniqueID.equal(ord)
                         // where o.StatusEnum == OrderStatus.BeSettle
                         select o).FirstOrDefault();
                //Log.print($"order ->{order.SerialNumber} uid->{order.UniqueID} equal ord->{ord}  {order.UniqueID.equal(ord)}");
                //order.saveLog("pay order data");
                if (order.isNullOrEmpty())
                    throw new ArgumentNullException();

                info = OrderManager.getPayInfo(order);


                //插入一筆序號來使用
                orderNo = OrderPayRecord.NewebPay(order.Id).Also(record => record.Insert()).EkiOrderSerial;



                member = (from m in GetTable<Member>()
                          where m.Id == order.MemberId
                          select m).FirstOrDefault();

            }
            catch (Exception ex)
            {
                //ex.saveLog("PayPage Error");
                Response.Redirect("~/Error/404");
            }
        }
    }
}