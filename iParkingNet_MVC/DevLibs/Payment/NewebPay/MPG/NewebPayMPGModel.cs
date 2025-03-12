using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayPostModel 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public abstract class NewebPayMPGModel
    {
        [NewebPaySet("MerchantID", true)]
        public string MerchantID { get; set; }
        [NewebPaySet("RespondType", true)]
        public string RespondType { get => NewebPayMPG.Config.RespondType; }
        [NewebPaySet("TimeStamp", true)]
        public string TimeStamp { get; set; }
        [NewebPaySet("Version", true)]
        public string Version { get => NewebPayMPG.Config.Version; }
        [NewebPaySet("LangType")]
        public string LangType { get; set; }
        [NewebPaySet("MerchantOrderNo", true)]
        public string MerchantOrderNo { get; set; }
        [NewebPaySet("Amt", true)]
        public int Amt { get; set; }
        [NewebPaySet("ItemDesc", true)]
        public string ItemDesc { get; set; }
        [NewebPaySet("ExpireDate")]//yyyyMMdd 最多間格為180天
        public string ExpireDate { get; set; }
        [NewebPaySet("ReturnURL")]
        public string ReturnURL { get; set; }
        [NewebPaySet("NotifyURL")]
        public string NotifyURL { get; set; }
        //[NewebPaySet("CustomerURL")]//先不要用
        //public string CustomerURL { get; set; }
        [NewebPaySet("ClientBackURL")]
        public string ClientBackURL { get; set; }
        [NewebPaySet("Email", true)]
        public string Email { get; set; }
        [NewebPaySet("LoginType", true)]
        public string LoginType { get => NewebPayMPG.Config.LoginType; }

        //[NewebPaySet("LINEPAY")]
        //public string LINEPAY { get { return PayConfig.藍新.LINEPAY; } }

        //[NewebPaySet("BARCODE")]
        //public string BARCODE { get { return PayConfig.藍新.BARCODE; } }


        /// <summary>
        /// 信用卡快速結帳=目前填入會員手機號碼
        /// 信用卡約定付款=可對應付款人之資料，用於綁定付款人與信用卡卡號時使用，例：會員編號、
        /// </summary>
        [NewebPaySet("TokenTerm")]
        public string TokenTerm { get; set; }



        /// <summary>
        /// 信用卡一次付清 物件
        /// </summary>
        public class CreditCard : NewebPayMPGModel
        {
            [NewebPaySet("Version", true)]
            public new string Version { get => NewebPayMPG.Config.Version; }

            [NewebPaySet("CREDIT")]//信用卡一次付清
            public string CREDIT { get; set; } = NewebPayMPG.Config.CREDIT;

            /**
             * 1 = 必填 信用卡 到期日與 背面 末三碼
                2 必填 信用卡 到期日
                3 = 必填 背面 末三碼
             * */
            [NewebPaySet("TokenTermDemand")]
            public string TokenTermDemand { get { return "3"; } }
        }


        /// <summary>
        /// 信用卡約定付款
        /// </summary>
        public class AgreeCredit:NewebPayMPGModel
        {
            [NewebPaySet("Version", true)]
            public new string Version { get => NewebPayMPG.Config.Version; }
            /// <summary>
            /// 1.設定於首次授權頁面，付款人電子信箱欄位是否開放讓付款人修改。
            ///1=可修改
            ///0=不可修改
            ///2.當未提供此參數時，將預設為可修改。
            /// </summary>
            [NewebPaySet("EmailModify",false)]
            public int EmailModify { get; set; } = 0;

            [NewebPaySet("CREDITAGREEMENT",true)]
            public int CREDITAGREEMENT { get => 1; }

            /// <summary>
            /// 約定事項
            /// 此參數內容將會於MPG頁面呈現給付款人，確認約定信用卡付款之約定事項。
            /// </summary>
            [NewebPaySet("OrderComment",true)]
            public string OrderComment { get; set; }


        }
    }
}
