using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SalesData 的摘要描述
/// </summary>
[DbTableSet("SalesData")]
public class SalesData : BaseDbDAO, IConvertResponse<object>
{
    [DbRowKey("Name")]
    public string Name { get; set; }
    [DbRowKey("Phone")]
    public string Phone { get; set; }
    [DbRowKey("Email")]
    public string Email { get; set; }
    [DbRowKey("Address")]
    public string Address { get; set; }
    [DbRowKey("ReferrerCode")]
    public string ReferrerCode { get; set; }
    [DbRowKey("AccountName")]
    public string AccountName { get; set; }
    [DbRowKey("BankCode")]
    public string BankCode { get; set; }
    [DbRowKey("BankName")]
    public string BankName { get; set; }
    [DbRowKey("BankSub")]
    public string BankSub { get; set; }
    [DbRowKey("BankAccount")]
    public string BankAccount { get; set; }
    [DbRowKey("Remarks")]
    public string Remarks { get; set; }
    [DbRowKey("CommissionSwitch")]
    public int CommissionSwitch { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("managerID")]
    public int managerID { get; set; }

    public object convertToResponse()
    => new
    {
        Name,
        Phone,
        ReferrerCode
    };

    public bool CreatByReferrerCode(string code)
    {
        return EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("ReferrerCode", code), this);
    }
    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}