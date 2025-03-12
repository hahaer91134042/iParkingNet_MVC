using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocMultiPageResponse 的摘要描述
/// </summary>
public class LocMultiPageResponse<T>:DataListModel<T>,ISocketMsg
{
    public int Page;
    public int Total;
    public double Lat;
    public double Lng;

    public string socketMethod() => EkiBroadCastMethod.LoadLocation.Name;
}