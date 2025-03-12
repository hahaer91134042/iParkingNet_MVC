using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

/// <summary>
/// EcPayResponse 的摘要描述
/// </summary>
public class EcPayResponse:IEcPayCheck
{
    [EcPayFeature]
    public string CustomField1 { get; set; }
    [EcPayFeature]
    public string CustomField2 { get; set; }
    [EcPayFeature]
    public string CustomField3 { get; set; }
    [EcPayFeature]
    public string CustomField4 { get; set; }
    [EcPayFeature]
    public string MerchantID { get; set; }
    [EcPayFeature]
    public string MerchantTradeNo { get; set; }
    [EcPayFeature]
    public string PaymentDate { get; set; }
    [EcPayFeature]
    public string PaymentType { get; set; }
    [EcPayFeature]
    public int PaymentTypeChargeFee { get; set; }//通路費
    [EcPayFeature]
    public int RtnCode { get { return (int)payResult; } set { payResult = value == 1 ? EcPayResult.Success : EcPayResult.Fail; } }
    [EcPayFeature]
    public string RtnMsg { get; set; }
    [EcPayFeature]
    public int SimulatePaid { get; set; }//是否為模擬付款
    [EcPayFeature]
    public string StoreID { get; set; }
    [EcPayFeature]
    public int TradeAmt { get; set; }
    [EcPayFeature]
    public string TradeDate { get; set; }
    [EcPayFeature]
    public string TradeNo { get; set; }
    [EcPayFeature(true)]
    public string CheckMacValue { get; set; }


    public EcPayResult payResult = EcPayResult.Fail;

    public string hashKey() => EcPayConfig.HashKey;

    public string hashIV() => EcPayConfig.HashIV;
}