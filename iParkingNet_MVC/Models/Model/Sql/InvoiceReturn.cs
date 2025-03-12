using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_NewebPay;

/// <summary>
/// InvoiceReturn 的摘要描述
/// </summary>
[DbTableSet("InvoiceReturn")]
public class InvoiceReturn : BaseDbDAO
{
    [DbRowKey("OrderId", false)]
    public int OrderId { get; set; }
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("InvoiceId")]
    public int InvoiceId { get; set; }
    [DbRowKey("InvoiceNumber")]
    public string InvoiceNumber { get; set; } = "";
    [DbRowKey("InvoiceTransNo")]
    public string InvoiceTransNo { get; set; }
    [DbRowKey("OrderNo")]
    public string OrderNo { get; set; }
    [DbRowKey("Status")]
    public string Status { get; set; }
    [DbRowKey("Message")]
    public string Message { get; set; }
    [DbRowKey("Result",false)]
    public string Result { get { return resultModel.toJsonString(); } set { resultModel = value.toObj<NewebPayInvoiceResult>(); } }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }

    public NewebPayInvoiceResult resultModel = new NewebPayInvoiceResult();

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public static InvoiceReturn Load(NewebPayInvoiceReturn model) => new InvoiceReturn
    {
        InvoiceNumber = model.Result.InvoiceNumber,
        InvoiceTransNo = model.Result.InvoiceTransNo,
        OrderNo = model.Result.MerchantOrderNo,
        Status = model.Status,
        Message = model.Message,
        resultModel = model.Result
    };
}