using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IMulctRule 的摘要描述
/// </summary>
public abstract class IMulctRule : IRuleCheck<TimeSpan>, IMulctSet
{
    public abstract double addtionalCompensation(IPriceSet<decimal> priceSet);
    public abstract double compensationRatio();
    public abstract bool isInRule(TimeSpan factor);
    public abstract double mulctRatio();
}