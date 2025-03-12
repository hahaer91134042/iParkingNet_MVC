using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderCancel 的摘要描述
/// </summary>
[DbTableSet("OrderCancel")]
public class OrderCancel : BaseDbDAO
{
    [DbRowKey("OrderId", false)]
    public int OrderId { get; set; }
    [DbRowKey("Cost")]
    public double Cost { get; set; } = 0;
    [DbRowKey("Text",DbAction.Update)]
    public string Text { get; set; }
    [DbRowKey("Paid", DbAction.Update)]
    public bool Paid { get; set; } = false;
    //[DbRowKey("byManager")]
    //public bool byManager { get; set; } = false;
    [DbRowKey("Time", RowAttribute.Time, DbAction.Update, true)]
    public DateTime Time { get; set; }
    [DbRowKey("Lat", DbAction.Update)]
    public double Lat { get; set; }
    [DbRowKey("Lng", DbAction.Update)]
    public double Lng { get; set; }
    [DbRowKey("Img",DbAction.Update)]
    public string Img { get; set; } = "";

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataByQueryPair(
            QueryPair.New().addQuery("OrderId", id), 
            this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {        
        return EkiSql.ppyp.update(this);
    }
}