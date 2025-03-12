using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// InvoiceResult 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayInvoiceResult
    {
        public string MerchantID { get; set; }
        public string InvoiceTransNo { get; set; }
        public string MerchantOrderNo { get; set; }
        public int TotalAmt { get; set; }
        public string InvoiceNumber { get; set; }
        public string RandomNum { get; set; }
        public string CreateTime { get; set; }
        public string CheckCode { get; set; }
        public string BarCode { get; set; }
        public string QRcodeL { get; set; }
        public string QRcodeR { get; set; }
    }
}
