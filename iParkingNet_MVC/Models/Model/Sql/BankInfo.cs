using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// BankInfo 的摘要描述
/// </summary>
[DbTableSet("BankInfo")]
public class BankInfo : BaseDbDAO,IConvertResponse<BankResponseModel>,IEdit<BankInfo>
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("AddressId",false)]
    public int AddressId { get; set; }//基本上這個ID跟member 裡面的要一樣
    [DbRowKey("Name", DbAction.Update)]
    public string Name { get; set; } = "";
    [DbRowKey("Salt", DbAction.Update)]
    public string Salt { get; set; }
    [DbRowKey("Bank", DbAction.Update)]
    public string Bank { get; set; }
    [DbRowKey("isPerson", DbAction.Update)]
    public bool isPerson { get; set; } = true;
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime sDate { get; set; }

    public BankSecretDecode bankDecode { 
        get {
            if (secret == null)
                secret = EkiEncoder.AES.decode<BankSecretDecode>(Bank, Salt);
            return secret;
        }
        set {
            secret = value;
            var crypto = EkiEncoder.AES.encode(secret);
            Salt = crypto.publicKey;
            Bank = crypto.cipher;
        }
    }
    private BankSecretDecode secret;

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }    
    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {
       return EkiSql.ppyp.update(this);
    }

    public BankResponseModel convertToResponse()
        => new BankResponseModel()
        {
            Name=Name,
            Key=Salt,
            Bank=Bank,
            IsPerson=isPerson,
            Address=(from a in EkiSql.ppyp.table<Address>()
                     where a.Id==AddressId
                     select a).FirstOrDefault()?.convertToResponse()
        };

    public void editBy(BankInfo data,int version=1)
    {
        Name = data.Name;
        bankDecode = data.bankDecode;
        isPerson = data.isPerson;
        beEnable = data.beEnable;
    }

}