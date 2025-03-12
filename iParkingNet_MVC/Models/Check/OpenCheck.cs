using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OpenCheck 的摘要描述
/// </summary>
public class OpenCheck
{
    public List<OpenTime> openList = new List<OpenTime>();

    //public OpenCheck(string date, List<OpenTime> weekOpenSet)
    //{
    //    weekOpenSet.ForEach(set =>
    //    {
    //        openList.Add(new OpenSet(TimeUtil.CombineToDateTime(date, set.StartTime),
    //            TimeUtil.CombineToDateTime(date, set.EndTime)));
    //    });
    //}
    public OpenCheck(IEnumerable<OpenTime> openTime)
    {
        openList.AddRange(openTime);
        //dayOpenTime.ForEach(set =>
        //{
        //    openList.Add(new OpenSet(set.getStartTime(), set.getEndTime()));
        //});
    }

    public bool anyOverLap(OpenTime other)
    {
        var weekOpen = (from o in openList
                        //
                        where o.weekEnum!=WeekEnum.NONE?o.getStartTime().DayOfWeek==other.getStartTime().DayOfWeek:
                        o.getStartTime().sameDay(other.getStartTime())//找到當天的時段來比對
                        select o);
        if (weekOpen.Count() > 0)
        {
            foreach(var open in weekOpen)
            {
                //open.saveLog(other.toJsonString()+"  isOverLap->"+ open.overlap(other));                
                //時間範圍完全涵蓋的情況
                if (open.overlap(other))
                    return true;
            }
        }        
        return false;
    }

    public bool IsOpen(ReservedTime time)
    {
        if (openList.Count < 1)
            return false;

        var start = time.getStartTime();
        var end = time.getEndTime();

        //假如預約時間 沒有包含在開放時間 就拒絕這預約
        return openList.Any(open => open.between(start) && open.between(end));
    }

    public bool IsOpen(DateTime time)
    {

        if (openList.Any(open => { return open.between(time); }))
            return true;

        //if (forbiList.Any(forbi => { return forbi.IsBetween(time); }))
        //    return true;

        //foreach (var open in openList)
        //{
        //    if (!open.IsBetween(time))
        //        return true;
        //}
        return false;
    }
}