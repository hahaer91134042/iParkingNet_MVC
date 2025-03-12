using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ForbiTimeRequest 的摘要描述
/// </summary>
public class OpenTimeRequest:RequestAbstractModel,IRequestConvert<OpenTime>
{
    public int week { get; set; } = -1;
    public string date { get; set; } = "";
    public string start { get; set; } = "";//24h制 hh:mm
    public string end { get; set; } = "";

    public OpenTime convertToDbModel()
    {
        return new OpenTime()
        {
            Week=week,
            Date=date,
            StartTime=start,
            EndTime=end
        };
    }

    public override bool isValid()
    {        
        var isDate = true;
        if (!date.Trim().isNullOrEmpty())//要加Trim避免有空格
            date.toDateTime(ApiConfig.DateFormat, ok =>
            {
                isDate = ok;
            });

        return week.containEnumValue<WeekEnum>() &&isDate
           && TextUtil.checkTimeFormat(start) && TextUtil.checkTimeFormat(end);

        //return (int)WeekEnum.NONE <= week && week <= (int)WeekEnum.Saturday && isDate
        //    &&TextUtil.checkTimeFormat(start) && TextUtil.checkTimeFormat(end);
    }

}