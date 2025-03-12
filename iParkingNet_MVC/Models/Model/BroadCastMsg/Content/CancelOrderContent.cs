using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CancelOrderContent 的摘要描述
/// </summary>
public class CancelOrderContent : IBroadCastMsg,IiosPushNotify
{
    public string fcmMethod() => EkiBroadCastMethod.CancelOrder.Name;

    public string socketMethod() => EkiBroadCastMethod.CancelOrder.Name;

    public string title() => EkiBroadCastMethod.CancelOrder.Name;

    public string body() => $"{EkiBroadCastMethod.CancelOrder.Name}Body";


    public string Name { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public string CarNum { get; set; }

}