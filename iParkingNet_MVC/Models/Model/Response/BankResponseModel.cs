using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BankResponseModel 的摘要描述
/// </summary>
public class BankResponseModel:ApiAbstractModel
{
    public string Name { get; set; }
    public string Key { get; set; }
    public string Bank { get; set; }
    public bool IsPerson { get; set; }
    public AddressResponseModel Address { get; set; }
}