using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayCreditModel 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayCreditModel
    {
        /// <summary>
        /// 時間戳記
        /// </summary>
        [NewebPaySet("TimeStamp", true)]
        public string TimeStamp { get; set; } = NewebPayUtil.timeStamp();
        [NewebPaySet("Version", true)]
        public string Version { get => NewebPayCreditCard.Config.Version; }

        /// <summary>
        /// 商店訂單編號
        /// </summary>
        [NewebPaySet("MerchantOrderNo", true)]
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        [NewebPaySet("Amt", true)]
        public int Amt { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        [NewebPaySet("ProdDesc", true)]
        public string ProdDesc { get; set; }

        /// <summary>
        /// 付款人電子信箱
        /// </summary>
        [NewebPaySet("PayerEmail",true)]
        public string PayerEmail { get; set; }

        /// <summary>
        /// 約定信用卡付款授權Token
        /// </summary>
        [NewebPaySet("TokenValue",true)]
        public string TokenValue { get; set; }

        /// <summary>
        /// 約定信用卡付款之付款人綁定資料
        /// </summary>
        [NewebPaySet("TokenTerm",true)]
        public string TokenTerm { get; set; }

        [NewebPaySet("TokenSwitch",true)]
        public string TokenSwitch { get => "on"; }

        /// <summary>
        /// 信用卡分期付款啟用
        /// </summary>
        [NewebPaySet("Inst")]
        public string Inst { get; set; } = "0";

        /// <summary>
        /// 是否為美國運通卡
        /// </summary>
        [NewebPaySet("CardAE")]
        public int CardAE { get; set; } = 0;
    }
}
