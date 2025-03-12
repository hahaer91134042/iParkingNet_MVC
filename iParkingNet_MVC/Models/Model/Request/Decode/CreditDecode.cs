using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CreditDecode 的摘要描述
/// </summary>
public class CreditDecode : RequestAbstractModel,
                                                IRequestConvert<MemberCredit>,
                                                IEncodeSet
{
    public int category { get; set; }
    public string cardNum { get; set; }
    public string limitDate { get; set; }
    public string check { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(cardNum);
            cleanXssStr(limitDate);
            cleanXssStr(check);
            cleanXssStr(firstName);
            cleanXssStr(lastName);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool isValid()
    {
        if (cleanXss())
        {
            return category <= (int)CreditCategory.MASTER && !string.IsNullOrEmpty(cardNum) && !string.IsNullOrEmpty(limitDate) &&
                !string.IsNullOrEmpty(check) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }
        return false;
    }
    public MemberCredit convertToDbModel()
    {

        return new MemberCredit()
        {
            CreditInfoDecode=new CreditInfoDecode()
            {
                Category = category,
                CardNum = cardNum,
                LimitDate = limitDate,
                CheckCode = check,
                FirstName = firstName,
                LastName = lastName
            },
            
            EndNum = cardNum.Substring(cardNum.Length - 4, 4),
            beEnable=true
        };
    }

    public IHashCodeSet hashSet() => EkiHashCode.SHA1;
}