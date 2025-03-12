using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Map_CancelTime_Mulct 的摘要描述
/// </summary>
[DbTableSet("Map_CancelTime_Mulct")]
public class Map_CancelTime_Mulct : BaseDbDAO
{
    [DbRowKey("MulctId")]
    public int MulctId { get; set; }
    [DbRowKey("CancelTimeId")]
    public int CancelTimeId { get; set; }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}