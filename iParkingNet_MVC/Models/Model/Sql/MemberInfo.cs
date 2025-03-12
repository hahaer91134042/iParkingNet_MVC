using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// MemberInfo 的摘要描述
/// </summary>
[DbTableSet("MemberInfo")]
public class MemberInfo : BaseDbDAO,IMapImg
{
    [DbRowKey("FirstName", DbAction.Update, true)]
    public string FirstName { get; set; }
    [DbRowKey("LastName", DbAction.Update, true)]
    public string LastName { get; set; }
    [DbRowKey("NickName",DbAction.Update,true)]
    public string NickName { get; set; }
    [DbRowKey("CountryCode",DbAction.Update,false)]
    public string CountryCode { get; set; }
    [DbRowKey("PhoneNum",DbAction.Update,false)]
    public string PhoneNum { get; set; }
    [DbRowKey("IconImg",DbAction.Update)]
    public string IconImg { get; set; }

    
    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public string imgName() => IconImg;

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {
       return EkiSql.ppyp.update(this);
    }
}