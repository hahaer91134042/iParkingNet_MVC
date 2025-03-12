using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BeManagerRequest 的摘要描述
/// </summary>
public class BankRequest : SecurityRequestModel<BankDecode>,
                                                  IRequestConvert<BankInfo>
{

    public AddressRequest address { get; set; } = new AddressRequest();

    public override bool isValid()
    {        
        try
        {
            if (base.isValid())
            {
                address?.notNull(a => a.cleanXss());
                return decode.isValid();
            }                
        }
        catch (Exception)
        {
        }
        return false;
    }

    public override BankDecode DecodeContent() => decodeAES();

    public BankInfo convertToDbModel()
    {

        return new BankInfo()
        {
            Name = decode.name,
            isPerson = decode.isPerson,
            bankDecode = new BankSecretDecode()
            {
                Serial = decode.serial,
                Code = decode.code,
                Account = decode.account,
                Sub=decode.sub
            }
        };
    }
}