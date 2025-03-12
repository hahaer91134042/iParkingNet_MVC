using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// InvoiceReturn 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayInvoiceReturn
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public NewebPayInvoiceResult Result { get; set; }


        //因為藍星的ezPay 的發票回傳的json很低能 弄了一堆\  所以無法解只能自己慢慢解string
        public class Parser
        {
            public static NewebPayInvoiceReturn parse(string raw)
            {
                var result = HttpUtility.UrlDecode(raw);
                var dic = new Dictionary<string, string>();
                var arrayLv1 = result.Split(Convert.ToChar("&"));
                foreach (var p in arrayLv1)
                {
                    var lv2 = p.Split(Convert.ToChar("="));
                    dic.Add(lv2[0], lv2[1]);
                }

                return new NewebPayInvoiceReturn
                {
                    Status = dic.get("Status"),
                    Message = dic.get("Message"),
                    Result = new NewebPayInvoiceResult
                    {
                        MerchantID = dic.get("MerchantID"),
                        InvoiceTransNo = dic.get("InvoiceTransNo"),
                        MerchantOrderNo = dic.get("MerchantOrderNo"),
                        TotalAmt = dic.get("TotalAmt").toInt(),
                        InvoiceNumber = dic.get("InvoiceNumber"),
                        RandomNum = dic.get("RandomNum"),
                        CreateTime = dic.get("CreateTime"),
                        CheckCode = dic.get("CheckCode"),
                        BarCode = dic.get("BarCode"),
                        QRcodeL = dic.get("QRcodeL"),
                        QRcodeR = dic.get("QRcodeR")
                    }
                };
            }

        }
    }
}
