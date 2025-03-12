using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AddressRequest 的摘要描述
/// </summary>
/**
 * [Country]
      ,[State]
      ,[City]
      ,[Detail]
      ,[ZipCode]
 * */
public class AddressRequest:RequestAbstractModel,IRequestConvert<Address>
{
    public string country { get; set; } = "";
    public string state { get; set; } = "";
    public string city { get; set; } = "";
    public string detail { get; set; } = "";
    public string zip { get; set; } = "";

    public Address convertToDbModel()
    {
        return new Address()
        {
            Country = country,
            State = state,
            City = city,
            Detail = detail,
            ZipCode = zip
        };
    }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(country);
            cleanXssStr(state);
            cleanXssStr(city);
            cleanXssStr(detail);
            cleanXssStr(zip);
            return true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    //暫時還用不到
    public override bool isValid()
    {
        return base.isValid();
    }

    public override bool isEmpty()
    {
        return string.IsNullOrEmpty(country) && string.IsNullOrEmpty(state) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(detail);
    }

}