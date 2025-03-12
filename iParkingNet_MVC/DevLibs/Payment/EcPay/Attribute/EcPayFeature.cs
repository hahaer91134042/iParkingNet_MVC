using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EcPayFeature 的摘要描述
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EcPayFeature:Attribute
{
    
    public bool isCheckCode;
    public EcPayFeature(bool isCheck = false)
    {
        isCheckCode = isCheck;
    }
}