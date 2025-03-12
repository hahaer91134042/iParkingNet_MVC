using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SysManager 的摘要描述
/// </summary>
[DbTableSet("sysManager")]
public class SysManager : BaseDbDAO
{
    [DbRowKey("managerID", RowAttribute.PrimaryKey, DbAction.Static, false)]
    public new int Id { get; set; }
    [DbRowKey("name", DbAction.Update,false)]
    public string name { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("ReferrerCode",DbAction.Update)]
    public string ReferrerCode { get; set; }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}