using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// OrderLinePay 的摘要描述
/// </summary>
[DbTableSet("OrderLinePay")]
public class OrderLinePay : BaseDbDAO
{
    [DbRowKey("RecordId")]
    public int RecordId { get; set; }
    [DbRowKey("TransactionId")]
    public long TransactionId { get; set; }
    [DbRowKey("ReserveResult", DbAction.Update)]
    public string ReserveResult { get; set; } = "";
    [DbRowKey("ConfirmResult", DbAction.Update)]
    public string ConfirmResult { get; set; } = "";

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
    public override bool Update() => EkiSql.ppyp.update(this);
}