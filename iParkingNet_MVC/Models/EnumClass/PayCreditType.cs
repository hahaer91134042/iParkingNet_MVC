using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PayCreditType 的摘要描述
/// </summary>
public class PayCreditType
{
    /// <summary>
    /// 基本信用卡付款(一次性)
    /// </summary>
    public const string Base = "0";

    /// <summary>
    /// 約定信用卡付款
    /// 幕後傳遞
    /// </summary>
    public const string Agree = "1";
}