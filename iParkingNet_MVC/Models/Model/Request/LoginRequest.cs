using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LoginRequest 的摘要描述
/// </summary>
public class LoginRequest:RequestAbstractModel
{
    public string acc { get; set; }
    public string pwd { get; set; }
    public string lan { get; set; }
    public int mobileType { get; set; }//預設0
    public string pushToken { get; set; }

    public bool isEmail = false;
    public bool isPhone = false;
    public override bool isValid()
    {
        isEmail = TextUtil.checkEMailVaild(acc);
        isPhone = !isEmail;

        var isLan = true;
        if (!string.IsNullOrEmpty(lan))
            isLan=Enum.IsDefined(typeof(LanguageFamily), lan);

        return (TextUtil.checkPwdVaild(pwd))&&isLan;
    }
    public override bool isEmpty()
    {
        if (!string.IsNullOrEmpty(acc) && !string.IsNullOrEmpty(pwd))
            return false;
        return true;
    }
}