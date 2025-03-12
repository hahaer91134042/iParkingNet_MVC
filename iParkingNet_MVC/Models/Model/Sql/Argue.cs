using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Comment 的摘要描述
/// </summary>
[DbTableSet("Argue")]
public class Argue : BaseDbDAO,IMapImg
{
    [DbRowKey("OrderId",false)]
    public int OrderId { get; set; }//申訴的訂單
    [DbRowKey("LocationId",false)]
    public int LocationId { get; set; }//申訴的地點
    [DbRowKey("MemberId",false)]
    public int MemberId { get; set; }//發起者的Id
    [DbRowKey("Source")]
    public int Source { get; set; }//發起者的身分
    [DbRowKey("Type", DbAction.Update)]
    public int Type { get; set; }//申訴類型
    [DbRowKey("Lat",DbAction.Update)]
    public double Lat { get; set; }
    [DbRowKey("Lng",DbAction.Update)]
    public double Lng { get; set; }
    [DbRowKey("Text", DbAction.Update)]
    public string Text { get; set; } = "";//內容
    [DbRowKey("Img",DbAction.Update)]
    public string Img { get; set; }//圖片
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }


    public ArgueSource commentSource { get { return Source.toEnum<ArgueSource>(); } set { Source = value.toInt(); } }
    public ArgueType commentType { get { return Type.toEnum<ArgueType>(); } set { Type = value.toInt(); } }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public string imgName() => Img;

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}