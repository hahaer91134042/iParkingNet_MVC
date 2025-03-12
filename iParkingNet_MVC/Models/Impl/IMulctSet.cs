using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IMulctSet 的摘要描述
/// </summary>
public interface IMulctSet
{
    //罰金比例
    double mulctRatio();
    //補償金比例
    double compensationRatio();
    //額外給的補償金
    double addtionalCompensation(IPriceSet<decimal> priceSet);
}