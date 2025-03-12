using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CheckoutResponse 的摘要描述
/// </summary>
public class CheckoutResponse : ApiAbstractModel
{
    public string Date { get; set; } = "";
    public double CostFix { get; set; } = 0;
    public double Claimant { get; set; } = 0;
    public string Img { get; set; } = "";
}