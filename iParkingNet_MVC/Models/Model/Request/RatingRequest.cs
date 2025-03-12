using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RatingRequest 的摘要描述
/// </summary>
public class RatingRequest:RequestAbstractModel
{
    public string serial { get; set; } = "";//目前是訂單流水號
    public double star { get; set; }
    public string text { get; set; }


    public override bool isValid()
    {
        star = Math.Round(star, 1, MidpointRounding.AwayFromZero);
        cleanXssStr(serial);
        cleanXssStr(text);
        return !serial.isNullOrEmpty()&&(star>=0d&& star<=5d);
    }
}