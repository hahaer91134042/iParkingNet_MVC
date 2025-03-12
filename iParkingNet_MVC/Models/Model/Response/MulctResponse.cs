using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MulctResponse 的摘要描述
/// </summary>
public class MulctResponse:ApiAbstractModel
{
    public double Amt { get; set; }
    public int Unit { get; set; }
    public bool Paid { get; set; }
    public string Time { get; set; }//紀錄地主哪個時間點產生的罰金(client)
}