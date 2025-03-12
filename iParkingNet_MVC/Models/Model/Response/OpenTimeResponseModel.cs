using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ForbiResponseModel 的摘要描述
/// </summary>
public class OpenTimeResponseModel:ApiAbstractModel
{
    public int Week = -1;
    public string Date = "";//yyyy-MM-dd
    public string StartTime = "";//24h制 hh:mm
    public string EndTime = "";

    public OpenTimeResponseModel() { }
    public OpenTimeResponseModel(OpenTime set)
    {
        Week = set.Week;
        Date = set.Date;
        StartTime = set.StartTime;
        EndTime = set.EndTime;
    }
}