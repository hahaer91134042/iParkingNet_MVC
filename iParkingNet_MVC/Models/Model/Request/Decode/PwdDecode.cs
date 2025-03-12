using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PwdDecode 的摘要描述
/// </summary>
public class PwdDecode:RequestAbstractModel,IEncodeSet
{
    public string phone { get; set; }
    public string pwd { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(phone);
            cleanXssStr(pwd);
            return true;
        }
        catch (Exception)
        {
        }
        return false;
    }  

    public override bool isValid()
    {
        if (cleanXss())
        {
            return !phone.isNullOrEmpty() && TextUtil.checkPwdVaild(pwd);
        }
        return false;
    }

    public IHashCodeSet hashSet() => EkiHashCode.SHA1;
}