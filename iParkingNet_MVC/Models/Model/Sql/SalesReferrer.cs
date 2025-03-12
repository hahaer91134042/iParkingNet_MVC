using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SalesReferrer 的摘要描述
/// 表示該member被哪個sales推薦
/// </summary>
[DbTableSet("SalesReferrer")]
public class SalesReferrer:BaseDbDAO
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }//被推薦人的Id
    [DbRowKey("SalesId",false)]
    public int SalesId { get; set; }//推薦人的SalesData Id
    [DbRowKey("ReferrerCode",false)]
    public string ReferrerCode { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}