using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// GoogleApi 的摘要描述
/// </summary>
public class GoogleApi
{
    public string Country="", State="", City="", Detail="";
    public string ApiUrl;


    public GoogleApi(AddressRequest address)
    {
        Country = address?.country;
        State = address?.state;
        City = address?.city;
        Detail = address?.detail;
        ApiUrl = getApiStr();
    }

    private string getApiStr()
    {
        var addrStr = new StringBuilder();
        addrStr.Append(string.IsNullOrEmpty(Country) ? "" : $"{Country}+");
        addrStr.Append(string.IsNullOrEmpty(State) ? "" : $"{State}+");
        addrStr.Append(string.IsNullOrEmpty(City) ? "" : $"{City}+");
        addrStr.Append(string.IsNullOrEmpty(Detail) ? "" : $"{Detail}+");
        if (addrStr.Length > 0)
            addrStr.Remove(addrStr.Length - 1, 1);

        return string.Format(ApiConfig.GoogleAddressUrl, HttpUtility.UrlEncode(addrStr.ToString()), ApiConfig.GoogleApiKey);
    }

    public static GoogleApi Parse(AddressRequest request)
    {
        return new GoogleApi(request);
    }
}