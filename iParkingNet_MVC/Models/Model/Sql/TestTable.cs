using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TestTable 的摘要描述
/// </summary>
[DbTableSet("TestTable1")]
public class TestTable : BaseDbDAO
{
    [DbRowKey("test")]
    public string test { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }

    public override bool CreatById(int id)
    {
       return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
}