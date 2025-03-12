using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[DbTableSet("LocationAD")]
public class LocationAD : BaseDbDAO, IConvertResponse<object>
{
    [DbRowKey("LocationId", false)]
    public int LocationId { get; set; }
    [DbRowKey("SalesId")]
    public int SalesId { get; set; }
    [DbRowKey("StartTime", RowAttribute.Time, DbAction.Update)]
    public DateTime StartTime { get; set; }
    [DbRowKey("EndTime", RowAttribute.Time, DbAction.Update)]
    public DateTime EndTime { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;//初始預設值
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("HtmlContent", DbAction.Update, true)]
    public string HtmlContent { get { return htmlDecode.toBase64(); } set { htmlDecode = value.decodeBase64(); } }
    [DbRowKey("managerID", DbAction.Update, false)]
    public int managerID { get; set; }

    //這是為了避免輸入的字串有空格導致錯誤所使用的
    public string htmlDecode = "";
    //這樣效率不好
    public object convertToResponse() => new
    {
        Id,
        Sales = new SalesData().Let(s =>
        {
            s.CreatById(SalesId);
            return s.convertToResponse();
        }),
        StartTime=StartTime.toString(),
        EndTime=EndTime.toString(),
        HtmlContent,
        beEnable
    };

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }
}