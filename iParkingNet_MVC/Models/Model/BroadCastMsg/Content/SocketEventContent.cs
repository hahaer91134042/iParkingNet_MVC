using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SocketEventContent 的摘要描述
/// </summary>
public class SocketEventContent : IBroadCastMsg
{
    public string fcmMethod() => EkiBroadCastMethod.SocketEvent.Name;

    public string socketMethod() => EkiBroadCastMethod.SocketEvent.Name;

    public static SocketEventContent Open = new SocketEventContent(1, "Socket open");
    public static SocketEventContent Close = new SocketEventContent(0, "Socket close");

    public int Event { get; set; }
    public string Msg { get; set; }
    SocketEventContent(int e, string m)
    {
        Event = e;
        Msg = m;
    }
}