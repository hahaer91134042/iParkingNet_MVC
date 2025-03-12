using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMulctRule_TwoWeek 的摘要描述
/// 地主兩周以上的罰金規則
/// A.	二周以上的車位預約，地主無須支付罰金即可取消該筆訂單。
/// </summary>
public class ManagerMulctRule_TwoWeek : IMulctRule
{
    public override bool isInRule(TimeSpan factor) => factor.TotalDays > 14;
    public override double compensationRatio() => 0d;
    public override double mulctRatio() => 0d;
    public override double addtionalCompensation(IPriceSet<decimal> priceSet) => 0d;
}