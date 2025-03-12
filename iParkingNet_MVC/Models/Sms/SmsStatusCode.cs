using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SmsStatusCode 的摘要描述
/// </summary>
public class SmsStatusCode
{
    public static SmsStatusCode Success = new SmsStatusCode("00000");
    public static SmsStatusCode PhoneError = new SmsStatusCode("00100");

    public string value;
    public SmsStatusCode(string v)
    {
        this.value = v;
    }
}