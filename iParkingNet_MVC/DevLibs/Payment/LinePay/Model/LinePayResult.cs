using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LinePayoutResp 的摘要描述
/// </summary>
namespace Eki_LinePayApi_v3
{
    public class LinePayResult
    {
        public class Reserve
        {
            public string returnCode { get; set; }
            public string returnMessage { get; set; }
            public Info info { get; set; }

            public class Info
            {
                public long transactionId { get; set; }
                public string paymentAccessToken { get; set; }
                public PaymentUrl paymentUrl { get; set; }
            }

            public class PaymentUrl
            {
                public string app { get; set; }
                public string web { get; set; }
            }
        }


        public class Confirm
        {
            public string returnCode { get; set; }
            public string returnMessage { get; set; }
            public Info info { get; set; }

            public class Info
            {
                public string orderId { get; set; }
                public long transactionId { get; set; }
                public string authorizationExpireDate { get; set; }
                public string regKey { get; set; }
                public PayInfo payInfo { get; set; }
                public Packages packages { get; set; }
            }

            public class PayInfo:List<PayInfo.Item>
            {
                public class Item
                {
                    public string method { get; set; }
                    public int amount { get; set; }
                    public string creditCardNickname { get; set; }
                    public string creditCardBrand { get; set; }
                    public string maskedCreditCardNumber { get; set; }

                    public string card4No()
                    {
                        return maskedCreditCardNumber.Substring(maskedCreditCardNumber.Length - 4, 4);
                    }
                }
            }

            public class Packages : List<Packages.Item>
            {
                public class Item
                {
                    public string id { get; set; }
                    public int amount { get; set; }
                    public int userFeeAmount { get; set; }
                }
            }
        }
    }
}
