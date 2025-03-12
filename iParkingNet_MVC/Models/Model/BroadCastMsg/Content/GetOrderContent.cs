using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GetOrderContent 的摘要描述
/// </summary>
public class GetOrderContent : IBroadCastMsg,IiosPushNotify
{
    public string fcmMethod() => EkiBroadCastMethod.GetOrder.Name;

    public string Name { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public string CarNum { get; set; }

    public static GetOrderContent load(EkiOrder order,Location loc) => new GetOrderContent
    {
        Name=loc.Info.InfoContent,
        Start=order.ReservaTime.StartTime.toString(),
        End=order.ReservaTime.EndTime.toString(),
        CarNum=order.CarNum
    };

    public string socketMethod() => EkiBroadCastMethod.GetOrder.Name;

    public string title() => EkiBroadCastMethod.GetOrder.Name;

    public string body() => $"{EkiBroadCastMethod.GetOrder.Name}Body";

}