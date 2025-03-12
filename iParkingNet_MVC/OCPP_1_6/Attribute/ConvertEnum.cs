using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ConvertEnum 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConvertEnum:Attribute
    {
        public Type type;
        public string key;
        public ConvertEnum(Type t,string k="")
        {
            if (!t.IsEnum) throw new ArgumentException("Type must be Enum Type!");
            type = t;
            key = k;
        }


    }
}
