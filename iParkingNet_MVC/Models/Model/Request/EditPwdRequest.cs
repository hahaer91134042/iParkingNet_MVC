using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EditPwdRequest 的摘要描述
/// </summary>
public class EditPwdRequest : SecurityRequestModel<EditPwdDecode>
{
    public override bool isValid()
    {
        try
        {
            if (base.isValid())
                return decode.isValid();
        }
        catch (Exception)
        {
        }
        return false;
    }

    public override EditPwdDecode DecodeContent() => decodeAES();
}