using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerGetPayContent 的摘要描述
/// </summary>
public class ManagerGetPayContent : IBroadCastMsg,IiosPushNotify
{
    public string OrderSerNum { get; set; }
    public decimal Cost { get; set; }
    public string fcmMethod() => EkiBroadCastMethod.ManagerGetPay.Name;


    public string body() => $"{EkiBroadCastMethod.ManagerGetPay.Name}Body";

    public string socketMethod() => EkiBroadCastMethod.ManagerGetPay.Name;

    public string title() => EkiBroadCastMethod.ManagerGetPay.Name;
}