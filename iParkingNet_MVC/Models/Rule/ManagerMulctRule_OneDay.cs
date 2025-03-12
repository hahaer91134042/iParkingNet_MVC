using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMulctRule_OneDay 的摘要描述
/// D.	地主因故須取消24小時至3天內的車位預約，地主需賠償預約訂單總額的三倍罰金。
/// 發給車主原預定停車費的1/2停車補償金
/// </summary>
public class ManagerMulctRule_OneDay :IMulctRule
{
    public override bool isInRule(TimeSpan factor) => 1 < factor.TotalDays && factor.TotalDays <= 3;
    public override double compensationRatio() => (1d / 2d);
    public override double mulctRatio() => 3d;
    public override double addtionalCompensation(IPriceSet<decimal> priceSet) => 0d;
}