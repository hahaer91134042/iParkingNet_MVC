using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderCancelContent 的摘要描述
/// </summary>
public class ManagerOrderCancelContent : IBroadCastMsg,IiosPushNotify
{
    public string fcmMethod() => EkiBroadCastMethod.ManagerCancelOrder.Name;

    public string socketMethod()=> EkiBroadCastMethod.ManagerCancelOrder.Name;

    public string title() => EkiBroadCastMethod.ManagerCancelOrder.Name;

    public string body() => $"{EkiBroadCastMethod.ManagerCancelOrder.Name}Body";


    public EkiOrderResponse Order { get; set; }
    //這邊之後要加入折價馬的response
    public DiscountResponse Discount { get; set; }
}