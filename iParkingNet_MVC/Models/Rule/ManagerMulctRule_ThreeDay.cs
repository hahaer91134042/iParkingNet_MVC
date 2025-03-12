using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMultRule_ThreeDay 的摘要描述
/// C.	地主因故須取消3天至一周內的車位預約，地主需賠償預約訂單總額的二倍罰金。
/// 發給車主原預定停車費的1/3停車補償金
/// </summary>
public class ManagerMulctRule_ThreeDay : IMulctRule
{
    public override bool isInRule(TimeSpan factor) => 3 < factor.TotalDays && factor.TotalDays <= 7;
    public override double compensationRatio() => (1d / 3d);
    public override double mulctRatio() => 2d;
    public override double addtionalCompensation(IPriceSet<decimal> priceSet) => 0d;
}