using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EditPwdDecode 的摘要描述
/// </summary>
public class EditPwdDecode : RequestAbstractModel, IEncodeSet
{
    public string oldPwd { get; set; }
    public string newPwd { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(oldPwd);
            cleanXssStr(newPwd);
            return true;
        }
        catch (Exception e)
        {
            e.saveLog("cleanXssError");
        }
        return false;
    }

    public override bool isValid()
    {
        if (cleanXss())
        {
            if (!oldPwd.isNullOrEmpty() &&! newPwd.isNullOrEmpty())
            if (TextUtil.checkPwdVaild(oldPwd) && TextUtil.checkPwdVaild(newPwd))
            {
                return !string.Equals(oldPwd,newPwd,StringComparison.Ordinal);
            }
        }
        return false;
    }

    public IHashCodeSet hashSet() => EkiHashCode.SHA1;
}