using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MemberInfoRequest 的摘要描述
/// </summary>
/**
 *       ,[FirstName]
      ,[LastName]
      ,[PhoneNum]
      ,[IconImg]
 * */
public class MemberInfoRequest:RequestAbstractModel
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string nickName { get; set; }
    public string countryCode { get; set; }
    public string phone { get; set; }
    //public string icon { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(firstName);
            cleanXssStr(lastName);
            cleanXssStr(nickName);
            cleanXssStr(countryCode);
            cleanXssStr(phone);
            return true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    public override bool isValid()
    {
        return (!string.IsNullOrEmpty(nickName)&& !string.IsNullOrEmpty(phone)&&!string.IsNullOrEmpty(countryCode));
    }
}