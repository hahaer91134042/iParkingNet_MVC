using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// FilterOpenSiteRule 的摘要描述
/// </summary>
public class FilterOpenSiteRule : IRuleCheck<OpenTime>
{
    private DateTime now;
    public FilterOpenSiteRule(DateTime t)
    {
        now = t;
    }

    public bool isInRule(OpenTime factor)
    {
        //週期性開放都要
        if (factor.weekEnum != WeekEnum.NONE)
            return true;
        else
        {
            //非週期性開放要找出該月份的開放清單
            var start = factor.getStartTime();
            if (start.Year > now.Year)
                return true;
            else if(start.Year==now.Year)
            {
                return start.Month >= now.Month;
            }
        }

        return false;
    }
}