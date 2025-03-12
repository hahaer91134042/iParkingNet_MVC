using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_NewebPay;

/// <summary>
/// PayConfig 的摘要描述
/// </summary>
public class PayConfig
{
    public const string PayPagePath = "/Page/Pay.aspx?ord={0}";
    //藍新的url似乎只能帶一組query string
    public const string PayReturnPath = "/Page/Return.aspx?ord={0}";
    public const string PayNotifyPath = "/Handler/NewebPayReceiver.ashx?ord={0}";

    

    public class Flag
    {
        public const string Eki_Pay_Credit = "Eki-Pay-Credit";
        public const string Eki_Pay_Data = "Eki-Pay-Data";
    }

    /// <summary>
    /// MPG
    /// </summary>
    public class 藍新
    {
        public static TestPay config() => new TestPay();
        //正式
        //public static FormalPay config() => new FormalPay();

        public class TestPay : INewebPayConfig,INewebPayBackUrl
        {
            public string hashIV() => "CSldFitbiFogBokP";
            public string hashKey() => "MFcY951ZD7Z7r0CS6kEZSRlCNBlIJMvy";
            public string merchantID() => "MS36975845";

            public string notifyUrl(params string[] args)
            {
                return $"{WebUtil.getWebURL()}{string.Format(PayNotifyPath,args)}";
            }

            public string returnUrl(params string[] args)
            {
                return $"{WebUtil.getWebURL()}{string.Format(PayReturnPath,args)}";
            }

            public string url() => "https://ccore.newebpay.com/MPG/mpg_gateway";
        }
        public class FormalPay : INewebPayConfig,INewebPayBackUrl
        {
            public string hashIV() => "PJxsOJOyQZe8HRYC";
            public string hashKey() => "n9ncIzBJfuqrEqOvJQnMGwL8jjjv9VRP";
            public string merchantID() => "MS3371888527";

            public string notifyUrl(params string[] args)
            {
                return $"{WebUtil.getWebURL()}{string.Format(PayNotifyPath, args)}";
            }

            public string returnUrl(params string[] args)
            {
                return $"{WebUtil.getWebURL()}{string.Format(PayReturnPath, args)}";
            }
            public string url() => "https://core.newebpay.com/MPG/mpg_gateway";    
        }

    }

    public class Invoice
    {
        public static TestInvoice config() => new TestInvoice();
        //正式
       // public static FormalInvoice config() => new FormalInvoice();

        public class TestInvoice : INewebPayConfig
        {
            public string hashIV() => "CEafAqeOU4LCGrzP";
            public string hashKey() => "9rQAFNQGOMdGyx6XoXJF0Q7Bd91qHsAg";
            public string merchantID() => "32589960";
            public string url() => "https://cinv.ezpay.com.tw/Api/invoice_issue";
        }

        public class FormalInvoice : INewebPayConfig
        {
            public string hashIV() => "PqqRILR6hli9Wy5C";
            public string hashKey() => "3oYCdpMrNi87iKn6gVM7lvdDGgNqjclI";
            public string merchantID() => "315131703";
            public string url() => "https://inv.ezpay.com.tw/Api/invoice_issue";
        }


        public const double TaxRate = 5.0;
        public const string ItemName = "會員停車費";
        public const string ItemUnit = "分鐘";

        public const string LoveCode = "7505";
    }

    public class CreditCard
    {
        public static TestPay config() => new TestPay();
        //正式
        //public static FormalPay config() => new FormalPay();

        public class TestPay : INewebPayConfig
        {
            public string hashIV() => "CSldFitbiFogBokP";
            public string hashKey() => "MFcY951ZD7Z7r0CS6kEZSRlCNBlIJMvy";
            public string merchantID() => "MS36975845";

            public string url() => "https://ccore.newebpay.com/API/CreditCard";
        }
        public class FormalPay : INewebPayConfig
        {
            public string hashIV() => "PJxsOJOyQZe8HRYC";
            public string hashKey() => "n9ncIzBJfuqrEqOvJQnMGwL8jjjv9VRP";
            public string merchantID() => "MS3371888527";

            public string url() => "https://core.newebpay.com/API/CreditCard";
        }
    }
   
}