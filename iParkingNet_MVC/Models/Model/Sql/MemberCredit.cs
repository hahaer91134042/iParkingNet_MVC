using DevLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MemberCredit 的摘要描述
/// </summary>
[DbTableSet("MemberCredit")]
public class MemberCredit : BaseDbDAO
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("PublicKey",false)]
    public string PublicKey { get; set; }
    [DbRowKey("EndNum",false)]
    public string EndNum { get; set; }
    [DbRowKey("CreditInfo",false)]
    public string CreditInfo { get; set; }
    [DbRowKey("beEnable",DbAction.Update)]
    public bool beEnable { get; set; }
    [DbRowKey("cDate",RowAttribute.CreatTime,true)]
    public DateTime cDate { get; set; }

    public CreditInfoDecode CreditInfoDecode = new CreditInfoDecode();

    public CreditCategory CategoryEnum { get { return convertIntToEnum<CreditCategory>(CreditInfoDecode.Category); } set { CreditInfoDecode.Category = convertEnumToInt(value); } }


    public override bool CreatById(int id)
    {
        try
        {
            EkiSql.ppyp.loadDataById(id, this);
            DecodeCreditInfo();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override int Insert(bool isReturnId = false)
    {
        EncodeCreditInfo();
        return EkiSql.ppyp.insert(this, isReturnId);
    }

    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }

    private void EncodeCreditInfo()
    {
        PublicKey = SecurityBuilder.CreateSaltKey();
        var hash = CreditInfoDecode.creatHash(PublicKey);
        //var hash = SecurityBuilder.CreateHashCode(EncryptFormat.SHA1, PublicKey, ApiConfig.JwtSecret);
        CreditInfo = CreditInfoDecode.encryptByAES(hash);
        //CreditInfo = SecurityBuilder.EncryptTextByAES(CreditInfoDecode.toJsonString(), hash);
    }

    public MemberCredit DecodeCreditInfo()
    {
        var hash = CreditInfoDecode.creatHash(PublicKey);
        //var hash = SecurityBuilder.CreateHashCode(EncryptFormat.SHA1, PublicKey, ApiConfig.JwtSecret);
        CreditInfoDecode = CreditInfo.decryptByAES<CreditInfoDecode>(hash);
       // CreditInfoDecode = JsonConvert.DeserializeObject<CreditInfoDecode>(SecurityBuilder.DecryptTextByAES(CreditInfo, hash));
        return this;
    }
}