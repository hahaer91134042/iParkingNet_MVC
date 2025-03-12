using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPaySet 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NewebPaySet : Attribute
    {
        public string Key;
        public bool IsNeed;
        public NewebPaySet(string key, bool isNeed = false)
        {
            Key = key;
            IsNeed = isNeed;
        }
    }
}
