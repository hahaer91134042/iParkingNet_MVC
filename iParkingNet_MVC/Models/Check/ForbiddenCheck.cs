using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ForbiddenCheck 的摘要描述
/// </summary>
public class ForbiddenCheck
{
    private List<Forbidden> forbiList = new List<Forbidden>();

    public ForbiddenCheck(List<ReservedTime> reservaTime)
    {
        reservaTime.ForEach(time =>
        {
            forbiList.Add(new Forbidden(time.getStartTime(), time.getEndTime()));
        });
    }

    public bool IsForbidden(ReservedTime time)
    {
        var start = time.getStartTime();
        var end = time.getEndTime();       

        foreach (var forbi in forbiList)
        {
            if (start <= forbi.Start && end >= forbi.End)//要預約的時段裡面已經包含已預約時間
                return true;
            if (forbi.IsBetween(start) || forbi.IsBetween(end))//剛好夾在中間的時段是不能預約
                return true;
        }
        return false;
    }

    public bool IsForbidden(DateTime time)
    {

        //if (openList.notAny(open => { return open.IsBetween(time); }))
        //    return true;

        //if (forbiList.Any(forbi => { return forbi.IsBetween(time); }))
        //    return true;

        //foreach (var open in openList)
        //{
        //    if (!open.IsBetween(time))
        //        return true;
        //}

        foreach (var forbi in forbiList)
        {
            if (forbi.IsBetween(time))
                return true;
        }
        return false;
    }
}