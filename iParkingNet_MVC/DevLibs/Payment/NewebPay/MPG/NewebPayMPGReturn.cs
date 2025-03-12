using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayMPGReturn 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayMPGReturn
    {

        public static NewebPayMPGReturn Load(HttpRequest request)
        {
            return new NewebPayMPGReturn(request);
        }

        NewebPayMPGReturn(HttpRequest request)
        {
            Status = request.Form["Status"];
            MerchantID = request.Form["MerchantID"];
            TradeInfo = request.Form["TradeInfo"];
            TradeSha = request.Form["TradeSha"];
        }


        public string Status { get; set; }
        public string MerchantID { get; set; }
        public string TradeInfo { get; set; }
        public string TradeSha { get; set; }
        //public string ResultStr { get; set; }
        public Result_MPG MPG { get; set; }

        public class Result_MPG
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public CreditCardResult Result { get; set; }

            public bool hasPayToken() => !Result.TokenValue.isNullOrEmpty() && !Result.TokenLife.isNullOrEmpty();
        }

        public class CreditCardResult
        {
            public string MerchantID { get; set; }
            public int Amt { get; set; }
            public string TradeNo { get; set; }
            public string MerchantOrderNo { get; set; }
            /**
             * CREDIT 信用卡 即時交易
                WEBATM WebATM 即時交易
                VACC ATM轉帳 非即時交易
                CVS 超商代碼繳費 非即時交易
                BARCODE 超商條碼繳費 非即時交易
                CVSCOM 超商取貨付款 非即時交易
             * */
            public string PaymentType { get; set; }
            public string PayTime { get; set; }
            public string IP { get; set; }
            /// <summary>
            /// 該筆 交易 款項保管銀行
            /// </summary>
            public string EscrowBank { get; set; }
            //---以下是信用卡才有的property---
            public string RespondCode { get; set; }

            /// <summary>
            /// 授權碼
            /// </summary>
            public string Auth { get; set; }
            /// <summary>
            /// 信用卡卡號前六碼
            /// </summary>
            public string Card6No { get; set; }
            /// <summary>
            /// 信用卡卡號後四碼
            /// </summary>
            public string Card4No { get; set; }
            /**
             * 1.3D回傳值 eci=1,2,5,6，代表為3D交易。
                2.若交易送至收單機構授權時已是失敗狀態，則本欄位的值會以空值回傳。
             * */
            public string ECI { get; set; }
            /**
             * 0=該筆交易為非使用信用卡快速結帳功能
               1=該筆交易為首次設定信用卡快速結帳功能
               2=該筆交易為使用信用卡快速結帳功能
               9=該筆交易為取消信用卡快速結帳功能功能
             * */
            public int TokenUseStatus { get; set; }

            /// <summary>
            /// 約定信用卡付款授權Token
            /// </summary>
            public string TokenValue { get; set; } = "";

            /// <summary>
            /// 約定信用卡付款授權之有效日期
            /// </summary>
            public string TokenLife { get; set; } = "";
            /**
             * CREDIT = 台灣發卡機構核發之信用卡
                FOREIGN = 國外發卡機構核發之卡
                UNIONPAY = 銀聯卡
                GOOGLEPAY = GooglePay
                SAMSUNGPAY = SamsungPay
                DCC = 動態貨幣轉換
             * */
            public string PaymentMethod { get; set; }
        }
    }
}
