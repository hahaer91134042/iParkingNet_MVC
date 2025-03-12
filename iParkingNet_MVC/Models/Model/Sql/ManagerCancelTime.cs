using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// 這是要紀錄地主取消掉的開放時間用的 
/// 普通的地主取消訂單不要使用
/// </summary>
[DbTableSet("ManagerCancelTime")]
public class ManagerCancelTime : BaseDbDAO,IEquatable<ManagerCancelTime>,ITimeMap
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }//紀錄地主ID
    [DbRowKey("Week")]
    public int Week { get { return convertEnumToInt(weekEnum); } set { weekEnum = convertIntToEnum<WeekEnum>(value); } }
    [DbRowKey("Date", true)]
    public string Date { get; set; }
    [DbRowKey("StartTime", true)]
    public string StartTime { get; set; }//24h制 hh:mm
    [DbRowKey("EndTime", true)]
    public string EndTime { get; set; }
    [DbRowKey("CancelTime", RowAttribute.Time,false)]
    public DateTime CancelTime { get; set; }

    public WeekEnum weekEnum = WeekEnum.NONE;

    public static bool operator ==(ManagerCancelTime o1, ManagerCancelTime o2) => o1.Equals(o2);
    public static bool operator !=(ManagerCancelTime o1, ManagerCancelTime o2) => !o1.Equals(o2);
    public bool Equals(ManagerCancelTime other) => this.equals(other);

    public static ManagerCancelTime initFrom(ITimeMap time) => new ManagerCancelTime()
    {
        Week=time.week(),
        Date=time.date(),
        StartTime=time.startTime(),
        EndTime=time.endTime()
    };

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
        return EkiSql.ppyp.update(this);
    }

    public int week() => Week;
    public string date() => Date;
    public string startTime() => StartTime;
    public string endTime() => EndTime;
}