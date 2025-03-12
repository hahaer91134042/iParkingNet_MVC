using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ActionResponse 的摘要描述
/// </summary>
public class ActionResponse:ApiAbstractModel
{
    public int Type { get; set; }
    public object Detail { get; set; }
    //public string End { get; set; }
}