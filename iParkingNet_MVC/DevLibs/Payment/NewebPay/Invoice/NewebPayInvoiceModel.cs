using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayInvoiceModel 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayInvoiceModel
    {
        [NewebPaySet("RespondType", true)]
        public string RespondType { get => NewebPayInvoice.Config.RespondType; }
        [NewebPaySet("Version", true)]
        public string Version { get => NewebPayInvoice.Config.Version; }
        [NewebPaySet("TimeStamp", true)]
        public string TimeStamp { get => NewebPayUtil.timeStamp(); }
        [NewebPaySet("TransNum", false)]
        public string TransNum { get; set; }
        [NewebPaySet("MerchantOrderNo", true)]
        public string MerchantOrderNo { get; set; }
        [NewebPaySet("Status", true)]
        public string Status { get; set; } = NewebPayInvoice.Status_instant;
        [NewebPaySet("Category", true)]
        public string Category { get; set; }
        [NewebPaySet("BuyerName", true)]
        public string BuyerName { get; set; }//買受人名稱
        [NewebPaySet("BuyerUBN", false)]
        public string BuyerUBN { get; set; }//買受人統一編號
        [NewebPaySet("BuyerAddress", false)]
        public string BuyerAddress { get; set; }//買受人地址
        [NewebPaySet("BuyerEmail", false)]
        public string BuyerEmail { get; set; }
        [NewebPaySet("CarrierType", false)]
        public string CarrierType { get; set; }
        [NewebPaySet("CarrierNum", false)]
        public string CarrierNum { get; set; }
        [NewebPaySet("LoveCode", false)]
        public string LoveCode { get; set; }
        [NewebPaySet("PrintFlag", true)]
        public string PrintFlag { get; set; } = NewebPayInvoice.PrintFlag_N;
        [NewebPaySet("TaxType", true)]
        public string TaxType { get; set; } = NewebPayInvoice.TaxType_Taxable;
        [NewebPaySet("TaxRate", true)]
        public double TaxRate { get; set; } = NewebPayInvoice.Config.TaxRate;
        [NewebPaySet("Amt", true)]
        public int Amt { get; set; }
        [NewebPaySet("AmtSales", false)]
        public int AmtSales { get; set; }
        [NewebPaySet("AmtZero", false)]
        public int AmtZero { get; set; }
        [NewebPaySet("AmtFree", false)]
        public int AmtFree { get; set; }
        [NewebPaySet("TaxAmt", true)]
        public int TaxAmt { get; set; }
        [NewebPaySet("TotalAmt", true)]
        public int TotalAmt { get; set; }

        [NewebPaySet("ItemName", true)]
        public string ItemName { get; set; }//商品名稱 
        [NewebPaySet("ItemCount", true)]
        public int ItemCount { get; set; } = 1;//商品數量 
        [NewebPaySet("ItemUnit", true)]
        public string ItemUnit { get; set; }//商品單位 
        [NewebPaySet("ItemPrice", true)]
        public int ItemPrice { get { return ItemAmt; } }//商品單價 =小計
        [NewebPaySet("ItemAmt", true)]
        public int ItemAmt { get; set; }//商品小計 =小計
        [NewebPaySet("Comment", true)]
        public string Comment { get; set; }//備註 因為信用卡的關係 所以必填末四碼

    }
}
