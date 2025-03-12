using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderExtendContent 的摘要描述
/// </summary>
public class OrderExtendContent : IBroadCastMsg,IiosPushNotify
{
    public string Serial { get; set; }
    public string End { get; set; }


    public static OrderExtendContent load(EkiOrder order)
    {

        return new OrderExtendContent
        {
            Serial = order?.SerialNumber,
            End = order?.getEndTime().toString()
        };
    }

    public string title() => EkiBroadCastMethod.OrderExtend.Name;
    public string body() => $"{EkiBroadCastMethod.OrderExtend.Name}Body";

    public string fcmMethod() => EkiBroadCastMethod.OrderExtend.Name;

    public string socketMethod() => EkiBroadCastMethod.OrderExtend.Name;


}