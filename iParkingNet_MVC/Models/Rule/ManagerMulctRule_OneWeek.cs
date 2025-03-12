using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMulctRule_OneWeek 的摘要描述
/// B.	地主因故須取消一周以上至二周內的車位預約，地主需賠償原預約訂單總額的罰金。
/// 發給車主原預定停車費的1/5停車補償金
/// </summary>
public class ManagerMulctRule_OneWeek : IMulctRule
{
    public override bool isInRule(TimeSpan factor) => 7 < factor.TotalDays && factor.TotalDays <= 14;
    public override double compensationRatio() => (1d/5d);
    public override double mulctRatio() => 1d;
    public override double addtionalCompensation(IPriceSet<decimal> priceSet) => 0d;
}