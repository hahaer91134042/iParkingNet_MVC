using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DiscountResponse 的摘要描述
/// </summary>
public class DiscountResponse:ApiAbstractModel
{
    public string Code { get; set; }
    public double Amt { get; set; }
    public bool IsRange { get; set; }
    public string End { get; set; }
}