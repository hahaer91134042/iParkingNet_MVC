using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StatusEnum 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class StatusEnum : ConvertEnum
    {
        public const string flag = "status";
        public StatusEnum(Type t) : base(t,flag)
        {
        }
    }
}
