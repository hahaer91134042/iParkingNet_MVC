using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderCheckResult 的摘要描述
/// </summary>
public class CheckOutContent : IBroadCastMsg,IiosPushNotify
{
    public string OrderSerNum { get; set; }
    public decimal Cost { get; set; }
    public int Unit { get; set; }
    public decimal HandlingFee { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get;set; }
    public string Img { get; set; }
    public string CarNum { get; set; }



    public string fcmMethod() => EkiBroadCastMethod.GetCheckOut.Name;
    public string socketMethod() => EkiBroadCastMethod.GetCheckOut.Name;

    public string body() => $"{EkiBroadCastMethod.GetCheckOut.Name}Body";
    public string title()=> EkiBroadCastMethod.GetCheckOut.Name;
}