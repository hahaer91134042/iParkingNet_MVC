using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EcPayConfig 的摘要描述
/// </summary>
public class EcPayConfig
{
    /**
     * 正式環境：https://payment.ecpay.com.tw/Cashier/AioCheckOut/V5
     * 測試環境：https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5
     * */
    public static string EcPayUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";
    public static string MerchantID = "2000132";
    public static string PlatformID = "";

    public static string HashKey = "5294y06JbISpM5x9";
    public static string HashIV = "v77hoKGq4kWxNNIS";

    public static string ReturnURL = "http://iparkingnet.eki.com.tw/handler/EcPayReceiver.ashx";//接收付款結果

    public static string ClientBackURL = "http://iparkingnet.eki.com.tw/page/ecpaytest.aspx";//顯示回到商店的url
    public static string OrderResultURL = "http://iparkingnet.eki.com.tw/page/ecpayback.aspx";//付款結果會帶回到設定的頁面 有設定會使ClientBackURL失效
}