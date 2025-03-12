using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using Eki_NewebPay;

/// <summary>
/// NewebPayReturn 的摘要描述
/// </summary>
[DbTableSet("NewebPayReturn")]
public class NewebPayReturn : BaseDbDAO
{
    [DbRowKey("OrderId",false)]
    public int OrderId { get; set; }
    [DbRowKey("Status")]
    public string Status { get; set; }
    [DbRowKey("Message")]
    public string Message { get; set; }
    [DbRowKey("MerchantID")]
    public string MerchantID { get; set; }
    [DbRowKey("MerchantOrderNo")]
    public string MerchantOrderNo { get; set; }
    [DbRowKey("Amt")]
    public int Amt { get; set; }
    [DbRowKey("TradeNo")]
    public string TradeNo { get; set; }
    [DbRowKey("Ip")]
    public string Ip { get; set; }
    /// <summary>
    /// 該筆 交易 款項保管銀行
    /// </summary>
    [DbRowKey("Bank")]
    public string Bank { get; set; }
    /**
        * CREDIT 信用卡 即時交易
           WEBATM WebATM 即時交易
           VACC ATM轉帳 非即時交易
           CVS 超商代碼繳費 非即時交易
           BARCODE 超商條碼繳費 非即時交易
           CVSCOM 超商取貨付款 非即時交易
        * */
    [DbRowKey("PaymentType")]
    public string PaymentType { get; set; }
    [DbRowKey("PayTime",RowAttribute.Time,true)]
    public DateTime PayTime { get; set; }
    [DbRowKey("RespondCode")]
    public string RespondCode { get; set; }
    [DbRowKey("Auth")]
    public string Auth { get; set; }
    /// <summary>
    /// 信用卡卡號前六碼
    /// </summary>
    [DbRowKey("Card6No")]
    public string Card6No { get; set; }
    /// <summary>
    /// 信用卡卡號後四碼
    /// </summary>
    [DbRowKey("Card4No")]
    public string Card4No { get; set; }
    /**
         * 0=該筆交易為非使用信用卡快速結帳功能
            1=該筆交易為首次設定信用卡快速結帳功能
            2=該筆交易為使用信用卡快速結帳功能
            9=該筆交易為取消信用卡快速結帳功能功能
         * */
    [DbRowKey("TokenUseStatus")]
    public int TokenUseStatus { get; set; }
    /**
       * 1.3D回傳值 eci=1,2,5,6，代表為3D交易。
          2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。
       * */
    [DbRowKey("ECI")]
    public string ECI { get; set; }
    /**
         * CREDIT = 台灣發卡機構核發之信用卡
            FOREIGN = 國外發卡機構核發之卡
            UNIONPAY = 銀聯卡
            GOOGLEPAY = GooglePay
            SAMSUNGPAY = SamsungPay
            DCC = 動態貨幣轉換
         * */
    [DbRowKey("PaymentMethod")]
    public string PaymentMethod { get; set; }


    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }

    public static NewebPayReturn Load(NewebPayCreditReturn model)
        => new NewebPayReturn
        {
            Status = model.Status,
            Message = model.Message,
            MerchantID = PayConfig.CreditCard.config().merchantID(),
            MerchantOrderNo = model.Result.MerchantOrderNo,
            Amt = model.Result.Amt,
            TradeNo = model.Result.TradeNo,
            Ip = model.Result.IP,
            Bank = model.Result.EscrowBank,
            PaymentType = model.Result.PaymentMethod,
            PayTime = model.Result.authTime(),
            RespondCode = model.Result.RespondCode,
            Auth = model.Result.Auth,
            Card6No = model.Result.Card6No,
            Card4No = model.Result.Card4No,
            TokenUseStatus = 1,
            ECI = model.Result.ECI,
            PaymentMethod = model.Result.PaymentMethod
        };

    public static NewebPayReturn Load(NewebPayMPGReturn model)
    {
        var mpg = model.MPG;
        return new NewebPayReturn()
        {
            Status = model.Status,
            MerchantID = model.MerchantID,
            MerchantOrderNo =mpg.Result.MerchantOrderNo,
            Message = mpg.Message,
            Amt = mpg.Result.Amt,
            TradeNo = mpg.Result.TradeNo,
            Ip = mpg.Result.IP,
            Bank = mpg.Result.EscrowBank,
            PaymentType = mpg.Result.PaymentType,
            PayTime = mpg.Result.PayTime.toDateTime(),
            RespondCode = mpg.Result.RespondCode,
            Auth = mpg.Result.Auth,
            Card6No = mpg.Result.Card6No,
            Card4No = mpg.Result.Card4No,
            TokenUseStatus = mpg.Result.TokenUseStatus,
            ECI = mpg.Result.ECI,
            PaymentMethod = mpg.Result.PaymentMethod
        };
    }
}