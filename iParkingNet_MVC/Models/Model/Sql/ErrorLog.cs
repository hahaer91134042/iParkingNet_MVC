using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ErrorLog 的摘要描述
/// </summary>
[DbTableSet("ErrorLog")]
public class ErrorLog : BaseDbDAO
{
    [DbRowKey("Api", DbAction.Update)]
    public string Api { get; set; }
    [DbRowKey("Input", DbAction.Update)]
    public string Input { get; set; }
    [DbRowKey("Exception", DbAction.Update)]
    public string Exception { get; set; }
    [DbRowKey("Msg", DbAction.Update)]
    public string Msg { get; set; }
    [DbRowKey("Ip", DbAction.Update)]
    public string Ip { get; set; }
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