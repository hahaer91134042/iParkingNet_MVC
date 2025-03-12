using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LinePayin 的摘要描述
/// Line Request Api Model
/// </summary>
namespace Eki_LinePayApi_v3
{
    public class LinePayReserve
    {
        public int amount { get => packages.Sum(p => p.amount); }
        public string currency { get; set; } = LinePay.Currency.TWD.ToString();
        public string orderId { get; set; }//伺服器訂單編號

        public Packages packages { get; set; } = new Packages();

        public RedirectUrl redirectUrls { get; set; } = new RedirectUrl();

        public Options options { get; set; } = Options.Normal;

        public class Options
        {
            public static Options Normal = new Options
            {
                payment=new Payment
                {
                    payType= "NORMAL"
                }
            };
            public static Options AutoPay = new Options
            {
                payment = new Payment
                {
                    payType = "PREAPPROVED"
                }
            };


            public Display display { get; set; }
            public Payment payment { get; set; }

            public class Payment
            {

                /// <summary>
                /// 是否自動請款
                ///true(預設)：呼叫Confirm API，統一進行授權/請款處理
                ///false：呼叫Confirm API只能完成授權，需要呼叫Capture API完成請款
                /// </summary>
                public bool capture { get; set; } = true;

                /// <summary>
                /// 付款類型
                ///NORMAL , 一般付款
                ///PREAPPROVED , 自動付款
                /// </summary>
                public string payType { get; set; }
            }

            public class Display
            {
                public string locale { get; set; } = "zh_TW";
                public bool checkConfirmUrlBrowser { get; set; } = false;
            }
        }

        public class RedirectUrl
        {
            public string confirmUrl { get; set; }
            public string cancelUrl { get; set; }
        }

        public class Packages : List<Package>
        {
            public new void Add(Package p)
            {
                p.id = (this.Count + 1).ToString();
                base.Add(p);
            }
        }

        public class Package
        {
            public string id { get; set; }
            public int amount { get => products.Sum(p => p.quantity * p.price); }
            public List<Product> products { get; set; } = new List<Product>();
        }

        public class Product
        {
            //order id or serial
            public string id { get; set; }
            public string name { get; set; }
            public string imageUrl { get; set; } = "";
            public int quantity { get; set; } = 1;//商品數量
            public int price { get; set; }//單價
        }


    }
}
