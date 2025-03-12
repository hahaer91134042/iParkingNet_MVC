using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LatLng 的摘要描述
/// </summary>
public class LatLng
{
    public double Lat, Lng;
    public LatLng(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }
    public LatLng(decimal lat, decimal lng)
    {
        Lat = Convert.ToDouble(lat);
        Lng = Convert.ToDouble(lng);
    }
}