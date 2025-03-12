using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TimeUtil 的摘要描述
/// </summary>
public class TimeUtil
{
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private const string format = "{0} {1}";

    public static long CombineToStamp(string dateStamp, string timeFragStr)
    {
        return toStamp(CombineToDateTime(dateStamp, timeFragStr));
    }

    public static string CombineToTimeStr(string dateFragStr, string timeFragStr)
    {
        return string.Format(format, dateFragStr, timeFragStr);
    }


    //public static string CombineStampToTimeStr(decimal dateStamp, string timeFragStr)
    //{
    //    return CombineStampToTimeStr(Convert.ToInt64(dateStamp), timeFragStr);
    //}
    //public static DateTime CombineStampToDateTime(decimal dateStamp, string timeFragStr)
    //{
    //    return CombineStampToDateTime(Convert.ToInt64(dateStamp), timeFragStr);
    //}
    public static DateTime CombineToDateTime(string date, string timeFragStr)
    {
        return CombineToTimeStr(date, timeFragStr).parse24Hour();
        //return DateTime.Parse(CombineToTimeStr(date, timeFragStr));
    }

    public static long toStamp(DateTime date)
    {

        //return Convert.ToInt64(date.Subtract(new DateTime(1970, 1, 1,0,0,0,0)).TotalMilliseconds);

        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    public static DateTime ToDateTime(long stamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(stamp);
    }

    //public static DateTime FromUnixMillis(long javaTimeStamp)
    //{
    //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //    return origin.AddMilliseconds(javaTimeStamp);
    //}
}