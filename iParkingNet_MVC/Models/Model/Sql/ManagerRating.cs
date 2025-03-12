using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Comment 的摘要描述
/// </summary>
[DbTableSet("ManagerRating")]
public class ManagerRating : BaseDbDAO
{
    [DbRowKey("OrderId",false)]
    public int OrderId { get; set; }//評分的訂單
    [DbRowKey("UserMemberId",false)]
    public int UserMemberId { get; set; }//評分的車主
    [DbRowKey("MemberId",false)]
    public int MemberId { get; set; }//發起者的Id
    [DbRowKey("Star",DbAction.Update)]
    public double Star { get; set; }
    [DbRowKey("Text", DbAction.Update)]
    public string Text { get; set; } = "";//內容
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }


    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
    public override bool Update() => EkiSql.ppyp.update(this);
}