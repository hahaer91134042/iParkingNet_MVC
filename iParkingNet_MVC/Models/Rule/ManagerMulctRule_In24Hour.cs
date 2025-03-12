using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMulctRule_In24Hour 的摘要描述
/// E.	地主因故須取消24小時內的車位預定，須賠償預約訂單總額的五倍罰金
/// 發給車主原預定停車費的全額停車費，且另加兩小時補償金
/// </summary>
public class ManagerMulctRule_In24Hour : IMulctRule
{
    public override bool isInRule(TimeSpan factor) => factor.TotalHours <= 24;

    public override double addtionalCompensation(IPriceSet<decimal> priceSet) => (120d / (double)priceSet.methodSet().Min) * priceSet.price().toDouble();

    public override double compensationRatio() => 1d;

    public override double mulctRatio() => 5d;
}