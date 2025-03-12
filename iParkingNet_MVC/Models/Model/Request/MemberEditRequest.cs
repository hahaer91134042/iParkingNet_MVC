using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// RegisterRequest 的摘要描述
/// </summary>
/**
 * ,[Mail]
      ,[PhoneNum]
      ,[Pwd]
      ,[SecuritySalt]
      ,[Ip]
      ,[beEnable]
      ,[UniqueID]
      ,[cDate]
      ,[uDate]
      ,[sDate]
      ,[eDate]
      ,[MobileType]
      ,[PushToken]
      ,[Lan]
      ,[AddressID]
      ,[InfoID]
 * */
public class MemberEditRequest : RequestAbstractModel
{
    public string mail { get; set; }
    public string phone { get; set; }
    //public string pwd { get; set; }
    //public bool beManager { get; set; }
    public AddressRequest address { get; set; }
    public MemberInfoRequest info { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(phone);
            return address!=null?address.cleanXss():true && info!=null?info.cleanXss():true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    public override bool isValid()
    {
        if (info != null)
        {
            if (TextUtil.checkEMailVaild(mail) && !string.IsNullOrEmpty(phone)&& info.isValid())
                return true;
        }
        return false;
    }

    public bool isOK()
    {
        var mailOk = true;
        mail.notNullOrEmpty(m => { mailOk = TextUtil.checkEMailVaild(mail); });        

        return mailOk;
    }

}