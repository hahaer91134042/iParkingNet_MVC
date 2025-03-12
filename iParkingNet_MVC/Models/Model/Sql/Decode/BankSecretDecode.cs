using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BankSecretDecode 的摘要描述
/// </summary>
public class BankSecretDecode : ApiAbstractModel, IEncodeSet
{
    public string Serial { get; set; } = "";
    public string Code { get; set; } = "";
    public string Account { get; set; } = "";
    public string Sub { get; set; } = "";


    public IHashCodeSet hashSet() => EkiHashCode.SHA1;
}