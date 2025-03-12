using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AddRequest 的摘要描述
/// </summary>
public class AddRequest : RequestAbstractModel
{
    public string code { get; set; }

   public bool isCodeNullOrEmpty()
    {
        cleanXssStr(code);
        return code.isNullOrEmpty();
    }

}