using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SerNumExtend 的摘要描述
/// </summary>
public static class SerNumExtend
{
    public static string generateLocSerialNum(this ISerialNumGenerator s)
    {
        do
        {
            var serNum = SerialNumUtil.LocationSerialNum();
            var count = EkiSql.ppyp.count<Location>(QueryPair.New().addQuery("SerNum", serNum));
            if (count < 1)
                return serNum;
        } while (true);
    }
    public static string generateOrderTimeSerNum(this ISerialNumGenerator s)
    {
        do
        {
            var now = DateTime.Now;
            var serNum = SerialNumUtil.OrderTimeSerialNum(now);
            var count = EkiSql.ppyp.count<EkiOrder>(QueryPair.New().addQuery("SerialNumber", serNum));
            if (count < 1)
                return serNum;
        } while (true);
    }
}