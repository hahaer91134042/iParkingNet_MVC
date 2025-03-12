using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiBroadCastMethod 的摘要描述
/// </summary>
public class EkiBroadCastMethod
{
    public static EkiBroadCastMethod GetCheckOut = new EkiBroadCastMethod("GetCheckOut");
    public static EkiBroadCastMethod ManagerCancelOrder = new EkiBroadCastMethod("ManagerCancelOrder");
    public static EkiBroadCastMethod GetOrder = new EkiBroadCastMethod("GetOrder");
    public static EkiBroadCastMethod CancelOrder = new EkiBroadCastMethod("CancelOrder");
    public static EkiBroadCastMethod SocketEvent = new EkiBroadCastMethod("SocketEvent");
    public static EkiBroadCastMethod ManagerGetPay = new EkiBroadCastMethod("ManagerGetPay");
    public static EkiBroadCastMethod OrderExtend = new EkiBroadCastMethod("OrderExtend");
    public static EkiBroadCastMethod LoadLocation = new EkiBroadCastMethod(SocketMethod.LoadLocation.ToString());

    public string Name;
    public EkiBroadCastMethod(string n)
    {
        Name = n;
    }
}