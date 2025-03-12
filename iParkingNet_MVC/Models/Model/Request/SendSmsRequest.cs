using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SendSmsRequest 的摘要描述
/// </summary>
public class SendSmsRequest:RequestAbstractModel,IPhoneMap
{
    public string lan { get { return Lan.ToString(); } set { Lan = value.toEnum<LanguageFamily>(); } }
    public string countryCode { get; set; }
    public string phone { get; set; }
    public bool isForget { get; set; } = false;

    public LanguageFamily Lan = LanguageFamily.TC;
    public override bool isEmpty()
    {
        //string.IsNullOrEmpty(countryCode) ||
        return string.IsNullOrEmpty(phone)||string.IsNullOrEmpty(lan);
    }
    public override bool isValid()
    {

        return TextUtil.isNumber(phone);
    }

    string IPhoneMap.countryCode() => countryCode;

    string IPhoneMap.phone() => phone;
}