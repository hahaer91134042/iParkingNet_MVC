using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GoogleMapResult 的摘要描述
/// </summary>
public class GoogleMapResult:ApiAbstractModel
{
    public List<MapResult> results { get; set; }
    public string status { get; set; }

    public class MapResult
    {
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public GLocation location { get; set; }
    }
    public class GLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public bool isSuccess()
    {
        return status == "OK";
    }
}