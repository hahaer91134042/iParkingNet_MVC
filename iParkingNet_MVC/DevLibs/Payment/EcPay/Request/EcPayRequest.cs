using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EcPayRequest 的摘要描述
/// </summary>
public  class EcPayRequest:IEcPayConnectSet
{
    [EcPayFeature]
    public string MerchantID { get { return EcPayConfig.MerchantID; } }
    [EcPayFeature]
    public string MerchantTradeNo { get; set; }
    [EcPayFeature]
    public string MerchantTradeDate { get { return tradeDate.ToString("yyyy/MM/dd HH:mm:ss"); } }
    [EcPayFeature]
    public string PaymentType { get { return "aio"; } }//這固定的
    //[EcPayFeature]
    //public string TimeStamp { get { return DateTime.Now.toUnixSecStamp().ToString(); } }
    [EcPayFeature]
    public int TotalAmount { get; set; }
    [EcPayFeature]
    public string TradeDesc { get { return HttpUtility.UrlEncode(tradeDesc); } }
    [EcPayFeature]
    public string ItemName { get { return itemName.toEcPayItemName(); } }
    [EcPayFeature]
    public string ReturnURL { get { return EcPayConfig.ReturnURL; } }
    [EcPayFeature]
    public string ChoosePayment { get { return payment.ToString(); } }
    [EcPayFeature(true)]
    public string CheckMacValue { get; set; }
    [EcPayFeature]
    public string ClientBackURL { get { return EcPayConfig.ClientBackURL; } }
    [EcPayFeature]
    public string OrderResultURL { get { return EcPayConfig.OrderResultURL; } }
    [EcPayFeature]
    public int EncryptType { get { return 1; } }//固定值



    public DateTime tradeDate = DateTime.Now;
    public string tradeDesc = "";
    public List<string> itemName = new List<string>();
    public EcPayment payment = EcPayment.Credit;

    public string url() => EcPayConfig.EcPayUrl;

    public string hashKey() => EcPayConfig.HashKey;

    public string hashIV() => EcPayConfig.HashIV;
}