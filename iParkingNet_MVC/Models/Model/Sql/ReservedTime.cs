using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// ForbiTime 的摘要描述
/// </summary>
[DbTableSet("ReservedTime")]
public class ReservedTime : BaseDbDAO,IConvertResponse<ReservaTimeResponse>,ITimeRange
{
    [DbRowKey("LocationId", false)]
    public int LocationId { get; set; }//目前是LocationId
    [DbRowKey("MemberId",false)]
    public int MemberId { get; set; }
    [DbRowKey("Week", DbAction.Update)]
    public int Week { get { return convertEnumToInt(weekEnum); } set { weekEnum = convertIntToEnum<WeekEnum>(value); } }
    //[DbRowKey("Date", RowAttribute.Time, DbAction.Update, true)]
    //public string Date { get; set; }
    [DbRowKey("StartTime", RowAttribute.Time,DbAction.Update, true)]
    public DateTime StartTime { get; set; }
    [DbRowKey("EndTime",RowAttribute.Time,DbAction.Update, true)]
    public DateTime EndTime { get; set; }
    [DbRowKey("CarNum", false)]
    public string CarNum { get; set; }
    [DbRowKey("Remark", DbAction.Update)]
    public string Remark { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("IsCancel",DbAction.Update, false)]
    public bool IsCancel { get { return cancelable; } set { cancelable = value; } }

    public WeekEnum weekEnum = WeekEnum.NONE;
    private bool cancelable = false;

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }
    public override bool Update()
    {
        //this.saveLog("ReservaTime update");
        return EkiSql.ppyp.update(this);
    }

    public ReservaTimeResponse convertToResponse()
    {
        return new ReservaTimeResponse()
        {
            //Week=Week,
            //Date=Date,
            StartTime=StartTime.toString(),
            EndTime=EndTime.toString(),
            Remark=Remark
        };
    }

    //public bool between(DateTime time)
    //{
    //    return StartTime <= time && time <= EndTime;
    //}

    public DateTime start() => StartTime;

    public DateTime end() => EndTime;
}