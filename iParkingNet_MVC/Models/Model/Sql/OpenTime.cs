using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// OpenTime 的摘要描述
/// </summary>
[DbTableSet("OpenTime")]
public class OpenTime : BaseDbDAO,IEquatable<OpenTime>,ITimeMap,IRange<DateTime>,IOverLap<OpenTime>
{
    [DbRowKey("ParentId", false)]//目前是ReservaConfig Id
    public int ParentId { get; set; }
    [DbRowKey("Week", DbAction.Update)]
    public int Week { get { return convertEnumToInt(weekEnum); } set { weekEnum = convertIntToEnum<WeekEnum>(value); } }
    [DbRowKey("Date", DbAction.Update, true)]
    public string Date { get; set; }
    [DbRowKey("StartTime", true)]
    public string StartTime { get; set; }//24h制 hh:mm
    [DbRowKey("EndTime", true)]
    public string EndTime { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }

    public WeekEnum weekEnum = WeekEnum.NONE;

    public static bool operator ==(OpenTime o1, OpenTime o2) => o1.Equals(o2);
    public static bool operator !=(OpenTime o1, OpenTime o2) => !o1.Equals(o2);
    public bool Equals(OpenTime other) => this.equals(other);


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

    public bool between(DateTime other)
    {
        //周期循環的情況
        if (weekEnum != WeekEnum.NONE)
        {
            if (Week == other.DayOfWeek.toInt())
            {
                //把現在這個週期時間轉成比較時間日期一樣
                var newOpen = new OpenTime().Also(o =>
                {
                    o.Date = other.ToString(ApiConfig.DateFormat);
                    o.StartTime = StartTime;
                    o.EndTime = EndTime;
                });
                return newOpen.getStartTime() <= other && other <= newOpen.getEndTime();
            }
            return false;
        }
        //非週期循環
        return this.getStartTime() <= other && other <= this.getEndTime();
    }

    public bool overlap(OpenTime other)
    {
        //確定都是相同日子
        if (Week == other.Week || this.getStartTime().DayOfWeek==other.getStartTime().DayOfWeek)
        {
            //相同週期 或者是 都是 非週期性時間
            if (Week == other.Week)
            {
                var start = other.getStartTime();
                var end = other.getEndTime();

                var thisStart = this.getStartTime();
                var thisEnd = this.getEndTime();
                //時間範圍完全涵蓋的情況or包在裡面
                if ((between(start) && between(end)) || (other.between(thisStart) && other.between(thisEnd)))
                    return true;
                //部分涵蓋(部分overlap)
                if ((thisStart < start && start < thisEnd) ||
                    (thisStart < end && end < thisEnd))
                    return true;
            }
            else//可能是剛好(週期時間的)週期數 根非週期的星期數一樣
            {
                var otherStart = other.getStartSpan();
                var otherEnd = other.getEndSpan();

                var thisStart = this.getStartSpan();
                var thisEnd = this.getEndSpan();

                if ((this.timeSpanBetween(otherStart) && this.timeSpanBetween(otherEnd)) ||
                    (other.timeSpanBetween(thisStart) && other.timeSpanBetween(thisEnd)))
                    return true;

                //部分涵蓋(部分overlap)
                if ((thisStart < otherStart && otherStart < thisEnd) ||
                    (thisStart < otherEnd && otherEnd < thisEnd))
                    return true;
            }
        }       

        return false;
    }
}