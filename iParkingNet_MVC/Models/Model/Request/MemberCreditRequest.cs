using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// MemberCreditRequest 的摘要描述
/// </summary>
public class MemberCreditRequest : SecurityRequestModel<CreditDecode>,
                                                                IRequestConvert<MemberCredit>
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

    public override CreditDecode DecodeContent()
    {
        return decodeAES();
    }
    public MemberCredit convertToDbModel()
    {
        return decode.convertToDbModel();
    }
}