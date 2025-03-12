using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LvPercent 的摘要描述
/// </summary>
[DbTableSet("LvPercent")]
public class LvPercent : BaseDbDAO
{
    [DbRowKey("ManagerLv")]
    public int ManagerLv { get; set; }
    [DbRowKey("AccountPercent")]
    public int AccountPercent { get; set; }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}