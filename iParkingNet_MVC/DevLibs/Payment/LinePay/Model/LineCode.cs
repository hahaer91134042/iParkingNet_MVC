using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LineCode 的摘要描述
/// </summary>
namespace Eki_LinePayApi_v3
{
    public class LineCode
    {
        public class Request
        {
            /// <summary>
            /// 成功
            /// </summary>
            public const string Success = "0000";

            /// <summary>
            /// 此商家不存在
            /// </summary>
            public const string MerchantNotExist = "1104";

            /// <summary>
            /// 此商家無法使用LINE Pay
            /// </summary>
            public const string LineCantUse = "1105";

            /// <summary>
            /// 標頭(Header)資訊錯誤
            /// </summary>
            public const string HeaderError = "1106";

            /// <summary>
            /// 金額有誤（scale）
            /// </summary>
            public const string AmountError = "1124";

            /// <summary>
            /// 正在進行付款
            /// </summary>
            public const string PayProgress = "1145";

            /// <summary>
            /// 該訂單編號(orderId)的交易記錄已經存在
            /// </summary>
            public const string OrderIdExist = "1172";

            /// <summary>
            /// 商家不支援該貨幣
            /// </summary>
            public const string CurrencyNotSupport = "1178";

            /// <summary>
            /// 付款金額不能小於 0
            /// </summary>
            public const string AmountLess0 = "1183";

            /// <summary>
            /// 此商家無法使用自動付款
            /// </summary>
            public const string AutoPayNotUse = "1194";

            /// <summary>
            /// 參數錯誤
            /// </summary>
            public const string ArgsError = "2101";

            /// <summary>
            /// JSON資料格式錯誤
            /// </summary>
            public const string JsonError = "2102";

            /// <summary>
            /// 內部錯誤
            /// </summary>
            public const string InnerError = "9000";
        }

        public class Check
        {
            /// <summary>
            /// 授權尚未完成
            /// </summary>
            public const string NotFinish = "0000";

            /// <summary>
            /// 授權完成 - 現在可以呼叫Confirm API
            /// </summary>
            public const string AuthFinish = "0110";

            /// <summary>
            /// 此商家不存在
            /// </summary>
            public const string MerchantNotExist = "1104";

            /// <summary>
            /// 此商家無法使用LINE Pay
            /// </summary>
            public const string LineCantUse = "1105";

            /// <summary>
            /// 該交易已被用戶取消，或者超時取消（20分鐘）- 交易已經結束了
            /// </summary>
            public const string Cancel = "0121";

            /// <summary>
            /// 付款失敗 - 交易已經結束了
            /// </summary>
            public const string Fail = "0122";

            /// <summary>
            /// 付款成功 - 交易已經結束了
            /// </summary>
            public const string Success = "0123";  

            /// <summary>
            /// 內部錯誤
            /// </summary>
            public const string InnerError = "9000";
        }

        public class Confirm
        {
            /// <summary>
            /// 成功
            /// </summary>
            public const string Success = "0000";

            /// <summary>
            /// 買家不是LINE Pay用戶
            /// </summary>
            public const string UnavlidUser = "1101";

            /// <summary>
            /// 1102	買方被停止交易
            /// </summary>
            public const string StopTrading = "1102";

            /// <summary>
            /// 此商家不存在
            /// </summary>
            public const string MerchantNotExist = "1104";

            /// <summary>
            /// 此商家無法使用LINE Pay
            /// </summary>
            public const string LineCantUse = "1105";

            /// <summary>
            /// 標頭(Header)資訊錯誤
            /// </summary>
            public const string HeaderError = "1106";

            /// <summary>
            /// 1110	無法使用的信用卡
            /// </summary>
            public const string UnvalidCard = "1110";

            /// <summary>
            /// 金額有誤（scale）
            /// </summary>
            public const string AmountError = "1124";

            /// <summary>
            /// 1141	付款帳戶狀態錯誤
            /// </summary>
            public const string AccountError = "1141";

            /// <summary>
            /// 1142	Balance餘額不足
            /// </summary>
            public const string InsufficientBalance = "1142";

            /// <summary>
            /// 1150	交易記錄不存在
            /// </summary>
            public const string RecordNotExist = "1150";

            /// <summary>
            /// 1152	該transactionId的交易記錄已經存在
            /// </summary>
            public const string RecordAlreadyExist = "1152";

            /// <summary>
            /// 1153	付款request時的金額與申請時的金額不一致
            /// </summary>
            public const string AmountsDoNotMatch = "1153";

            /// <summary>
            /// 1159	無付款申請資訊
            /// </summary>
            public const string NoPayReq = "1159";

            /// <summary>
            /// 1169	用來確認付款的資訊錯誤（請訪問LINE Pay設置付款方式與密碼認證）
            /// </summary>
            public const string CheckPayInfoError = "1169";

            /// <summary>
            /// 1170	使用者帳戶的餘額有變動
            /// </summary>
            public const string BalanceHasChanged = "1170";

            /// <summary>
            /// 該訂單編號(orderId)的交易記錄已經存在
            /// </summary>
            public const string OrderIdExist = "1172";

            /// <summary>
            /// 商家不支援該貨幣
            /// </summary>
            public const string CurrencyNotSupport = "1178";

            /// <summary>
            /// 1180	付款時限已過
            /// </summary>
            public const string DeadlineHasExpired = "1180";

            /// <summary>
            /// 付款金額不能小於 0
            /// </summary>
            public const string AmountLess0 = "1183";

            /// <summary>
            /// 1198	API調用重覆
            /// </summary>
            public const string ApiRepeat = "1198";

            /// <summary>
            /// 1199	內部請求錯誤
            /// </summary>
            public const string InnerReqError = "1199";

            /// <summary>
            /// 1264	一卡通MONEY通相關錯誤
            /// </summary>
            public const string OneCardMoneyError = "1264";

            /// <summary>
            /// 1280	信用卡付款時候發生了臨時錯誤
            /// </summary>
            public const string CreditCardTempError = "1280";

            /// <summary>
            /// 1281	信用卡付款錯誤
            /// </summary>
            public const string CreditCardPayError = "1281";

            /// <summary>
            /// 1282	信用卡授權錯誤
            /// </summary>
            public const string CreditCardAuthError = "1282";

            /// <summary>
            /// 1283	因有異常交易疑慮暫停交易，請洽 LINE Pay 客服確認
            /// </summary>
            public const string AbnormalTransaction = "1283";

            /// <summary>
            /// 1284	暫時無法以信用卡付款
            /// </summary>
            public const string CardCantPay = "1284";

            /// <summary>
            /// 1285	信用卡資訊不完整
            /// </summary>
            public const string CardInfoIncomplete = "1285";

            /// <summary>
            /// 1286	信用卡付款資訊不正確
            /// </summary>
            public const string CardInfoIncorrect = "1286";

            /// <summary>
            /// 1287	信用卡已過期
            /// </summary>
            public const string CreditCardExpired = "1287";

            /// <summary>
            /// 1288	信用卡的額度不足
            /// </summary>
            public const string InsufficientCreditCardLimit = "1288";

            /// <summary>
            /// 1289	超過信用卡付款金額上限
            /// </summary>
            public const string ExceededCreditCardPayment = "1289";

            /// <summary>
            /// 1290	超過一次性付款的額度
            /// </summary>
            public const string ExceedOneTimePayment = "1290";

            /// <summary>
            /// 1291	此信用卡已被掛失
            /// </summary>
            public const string CreditCardLost = "1291";

            /// <summary>
            /// 1292	此信用卡已被停卡
            /// </summary>
            public const string CreditCardSuspended = "1292";

            /// <summary>
            /// 1293	信用卡驗證碼 (CVN) 無效
            /// </summary>
            public const string CardCVNInvalid = "1293";

            /// <summary>
            /// 1294	此信用卡已被列入黑名單
            /// </summary>
            public const string CardInBlacklist = "1294";

            /// <summary>
            /// 1295	信用卡號無效
            /// </summary>
            public const string CardInvalid = "1295";

            /// <summary>
            /// 1296	無效的金額
            /// </summary>
            public const string AmountInvalid = "1296";

            /// <summary>
            /// 1298	信用卡付款遭拒絕
            /// </summary>
            public const string CardPaymentDeclined = "1298";

            /// <summary>
            /// 內部錯誤
            /// </summary>
            public const string InnerError = "9000";
        }
    }
}
