using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using Eki_NewebPay;

namespace iParkingNet_MVC.Handler
{
    /// <summary>
    /// NewebPayReceiver 的摘要描述
    /// </summary>
    public class NewebPayReceiver : BaseHandler, ISerialNumGenerator
    {
        override protected void GetRequest(HttpContext context, SqlContext sqlHelper)
        {
            var ord = context.Request["ord"];

            var order = (from o in EkiSql.ppyp.table<EkiOrder>()
                         where o.beEnable
                         where o.UniqueID.equal(ord)
                         //where o.StatusEnum == OrderStatus.BeSettle
                         select o).FirstOrDefault();
            if (order.isNullOrEmpty())
                throw new ArgumentNullException();

            Log.d($"NewebPay Receiver order->{order.SerialNumber}");

            try
            {

                var returnModel = NewebPayMPGReturn.Load(context.Request);

                if (returnModel.Parse())
                {
                    if (returnModel.MPG.hasPayToken())
                    {
                        var mPayInfo = EkiSql.ppyp.data<MemberPayInfo>(
                            "where MemberId=@mid",
                            new { mid = order.MemberId });
                        if (mPayInfo == null)
                        {
                            mPayInfo = new MemberPayInfo
                            {
                                MemberId = order.MemberId,
                                neweb = new MemberPayInfo.Info_Neweb
                                {
                                    Token = returnModel.MPG.Result.TokenValue,
                                    TokenLife = returnModel.MPG.Result.TokenLife
                                }
                            };

                            mPayInfo.Insert();
                        }
                        else
                        {
                            mPayInfo.neweb.Token = returnModel.MPG.Result.TokenValue;
                            mPayInfo.neweb.TokenLife = returnModel.MPG.Result.TokenLife;
                            mPayInfo.Update();
                        }
                    }

                    NewebPayReturn.Load(returnModel).Also(rModel =>
                    {
                        rModel.OrderId = order.Id;
                        rModel.Insert();

                        new OrderCheckOutProcess(order)
                        {
                            checkSuccess = () => returnModel.Status == "SUCCESS",
                            tradeNo = () => rModel.TradeNo,
                            card4No = () => rModel.Card4No
                        }.run();

                    });

                }
                else
                {
                    //rBuilder.Append("Can`t parse model");
                    //PayStatusText.Text = "交易結果出現異常";
                }
            }
            catch (Exception e)
            {

                Log.e("NewebPay Receiver error", e);
            }

        }
    }
}