using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservaTimeResponse 的摘要描述
/// </summary>
public class ReservaTimeResponse:ApiAbstractModel
{
    //public int Week { get; set; }
   // public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Remark { get; set; }
    public bool IsUser = false;


    public ReservaTimeResponse setIsUser(bool b)
    {
        IsUser = b;
        return this;
    }

}