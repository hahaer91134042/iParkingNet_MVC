using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_NewebPay;

/// <summary>
/// EkiInvoice 的摘要描述
/// </summary>
[DbTableSet("Invoice")]
public class EkiInvoice : BaseDbDAO
{
    [DbRowKey("OrderId", false)]
    public int OrderId { get; set; }
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("Type")]
    public int Type { get { return type.toInt(); } set { type = value.toEnum<InvoiceType>(); } }
    [DbRowKey("SerNO")]
    public string SerNO { get; set; } = "";//基本上是訂單編號用來傳給ezPay的
    [DbRowKey("Name")]
    public string Name { get; set; } = "";
    [DbRowKey("Address")]
    public string Address { get; set; } = "";
    [DbRowKey("Email")]
    public string Email { get; set; } = "";
    [DbRowKey("BuyerUBN")]
    public string BuyerUBN { get; set; } = "";
    [DbRowKey("CarrierNum")]
    public string CarrierNum { get; set; } = "";
    [DbRowKey("LoveCode")]
    public string LoveCode { get; set; } = PayConfig.Invoice.LoveCode;

    public InvoiceType type = InvoiceType.None;

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);


    public string payCarrierType()
    {
        //B2B不能
        if (!BuyerUBN.isNullOrEmpty())
            return "";

        var t = "";

        switch (type)
        {
            case InvoiceType.Phone:
                t = NewebPayInvoice.CarrierType_phone;
                break;
            case InvoiceType.Certificate:
                t = NewebPayInvoice.CarrierType_person;
                break;
            case InvoiceType.ezPay:
                t = NewebPayInvoice.CarrierType_ezPay;
                break;
        }

        return t;
    }
}