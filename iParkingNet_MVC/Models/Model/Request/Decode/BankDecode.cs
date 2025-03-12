using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BankDecode 的摘要描述
/// </summary>
public class BankDecode:RequestAbstractModel,IEncodeSet
{
    public string name { get; set; }
    public bool isPerson { get; set; } = true;
    public string serial { get; set; }
    public string code { get; set; }//銀行代碼
    public string sub { get; set; }//分行代碼
    public string account { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(name);
            cleanXssStr(serial);
            cleanXssStr(code);
            cleanXssStr(account);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool isValid()
    {
        if(cleanXss())
        return !name.isNullOrEmpty() && !serial.isNullOrEmpty() && 
            !code.isNullOrEmpty() && !account.isNullOrEmpty();
        return false;
    }

    public IHashCodeSet hashSet() => EkiHashCode.SHA1;
}