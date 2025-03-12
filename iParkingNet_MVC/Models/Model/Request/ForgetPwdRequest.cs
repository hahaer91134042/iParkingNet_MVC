using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ForgetPwdRequest 的摘要描述
/// </summary>
public class ForgetPwdRequest:SecurityRequestModel<PwdDecode>
{
    public override PwdDecode DecodeContent() => decodeAES();

    public override bool isValid()
    {        
        try
        {
            if (base.isValid())
                return decode.isValid();
        }
        catch (Exception) { }
        return false;
    }
}