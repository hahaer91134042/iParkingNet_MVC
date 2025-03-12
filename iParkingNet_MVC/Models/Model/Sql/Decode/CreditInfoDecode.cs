using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CreditInfoDecode 的摘要描述
/// </summary>
public class CreditInfoDecode:ApiAbstractModel,IEncodeSet
{
    public int Category { get; set; }
    public string CardNum { get; set; }
    public string LimitDate { get; set; }
    public string CheckCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public IHashCodeSet hashSet() => EkiHashCode.SHA1;

}