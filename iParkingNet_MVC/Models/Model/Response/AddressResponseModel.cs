using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AddressResponseModel 的摘要描述
/// </summary>
public class AddressResponseModel:ApiAbstractModel
{
    public string Country = "";
    public string State = "";
    public string City = "";
    public string Detail = "";
    public string Zip = "";

    public void load(Address data)
    {
        Country = data.Country;
        State = data.State;
        City = data.City;
        Detail = data.Detail;
        Zip = data.ZipCode;
    }
}