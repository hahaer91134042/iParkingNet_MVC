using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// UnitEnum 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class UnitEnum : ConvertEnum
    {
        public const string flag = "unit";
        public UnitEnum(Type t) : base(t, flag)
        {
        }
    }
}
