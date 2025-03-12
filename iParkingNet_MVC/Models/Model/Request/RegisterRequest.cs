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
public class RegisterRequest : RequestAbstractModel,IRequestConvert<Member>
{
    public string mail { get; set; }
    public string phone { get; set; }
    public string pwd { get; set; }
    public int mobileType { get; set; }
    public string pushToken { get; set; }
    public string lan { get; set; }
    public bool beManager { get; set; }
    public AddressRequest address { get; set; }
    public MemberInfoRequest info { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(phone);
            cleanXssStr(lan);
            return address!=null?address.cleanXss():true && info!=null?info.cleanXss():true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    public Member convertToDbModel()
    {
        if (cleanXss())
        {
            //var saltKey = SecurityBuilder.CreateSaltKey(5);
            //var hashPwd = SecurityBuilder.CreatePasswordHash(saltKey, pwd, EncryptFormat.SHA256);

            var member = new Member();
            member.creatCipher(pwd);

            member.Mail = mail;
            member.PhoneNum = phone;
            //member.SecuritySalt = saltKey;
            //member.Pwd = hashPwd;
            member.MobileType = mobileType;
            member.Lan = lan;
            member.PushToken = pushToken;
            member.Ip = WebUtil.GetUserIP();
            member.beEnable = true;
            member.beManager = beManager;

            if (address != null)
            {
                member.Address.Country = address.country;
                member.Address.City = address.city;
                member.Address.State = address.state;
                member.Address.ZipCode = address.zip;
                member.Address.Detail = address.detail;
            }
            member.Info.FirstName = info.firstName;
            member.Info.LastName = info.lastName;
            member.Info.CountryCode = info.countryCode;
            member.Info.PhoneNum = info.phone;
            member.Info.NickName = info.nickName;

            return member;
        }
        else
        {
            throw new ArgumentException("Register obj Error!");
        }
    }

    public override bool isValid()
    {
        if (info != null)
        {
            if (TextUtil.checkEMailVaild(mail) && !string.IsNullOrEmpty(phone) && TextUtil.checkPwdVaild(pwd) && info.isValid())
                return true;
        }
        return false;
    }

}