using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// MemberInfo 的摘要描述
/// </summary>
[DbTableSet("Member")]
public class Member : BaseDbDAO, IToken, ICryptoPwd, IPushSet, IConvertResponse<MemberResponse>, IPhoneMap
{
    [DbRowKey("Mail", DbAction.Update, false)]
    public string Mail { get; set; }
    [DbRowKey("PhoneNum", DbAction.Update, false)]
    public string PhoneNum { get; set; }
    [DbRowKey("Pwd", DbAction.Update, false)]
    public string Pwd { get; set; }
    [DbRowKey("SecuritySalt", DbAction.Update)]
    public string SecuritySalt { get; set; }
    [DbRowKey("Ip", DbAction.Update, true)]
    public string Ip { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; }
    [DbRowKey("beManager", DbAction.Update)]
    public bool beManager { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime sDate { get; set; }
    [DbRowKey("eDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime eDate { get; set; }
    [DbRowKey("MobileType", DbAction.Update, true)]
    public int MobileType { get { return convertEnumToInt(mobilType); } set { mobilType = convertIntToEnum<MobilType>(value); } }
    [DbRowKey("PushToken", DbAction.Update)]
    public string PushToken { get; set; }
    [DbRowKey("Lan", DbAction.Update, true)]
    public string Lan { get { return convertEnumToString(memberLan); } set { memberLan = convertStringToEnum<LanguageFamily>(value); } }
    [DbRowKey("AddressID")]
    public int AddressID { get; set; }
    [DbRowKey("InfoID")]
    public int InfoID { get; set; }
    [DbRowKey("Lv", DbAction.Update)]
    public int Lv { get; set; } = 0;//車主等級
    [DbRowKey("ManagerLv", DbAction.Update)]
    public int ManagerLv { get; set; } = 0;//地主等級


    public MobilType mobilType = MobilType.Android;
    public LanguageFamily memberLan = LanguageFamily.TC;

    public Address Address
    {
        get
        {
            if (addr == null)
                addr = new Address().Also(a => a.CreatById(AddressID));
            return addr;
        }
        //set { addr = value; }
    }
    public MemberInfo Info
    {
        get
        {
            if (info == null)
                info = new MemberInfo().Also(i => i.CreatById(InfoID));
            return info;
        }
        //set { info = value; }
    }
    public MemberPayInfo PayInfo
    {
        get
        {
            if (_pInfo == null)
                _pInfo = EkiSql.ppyp.data<MemberPayInfo>(
                    "where MemberId=@mid",
                    new { mid = Id });
            return _pInfo;
        }
    }


    private Address addr;
    private MemberInfo info;
    private MemberPayInfo _pInfo;

    public static QueryPair checkValidMemberQuery(string key, string value)
        => QueryPair.New()
                .addQuery(key, value)
                .addQuery("beEnable", 1);

    public MemberInfo mapIconUrlInfo()
    {
        if (!string.IsNullOrEmpty(Info.IconImg))
            Info.IconImg = $"{WebUtil.getWebURL()}{DirPath.Member}/{UniqueID.toString()}/{Info.IconImg}";
        return Info;
    }

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }
    public override bool CreatByUniqueId(string uniqueId)
    {
        return EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("UniqueID", uniqueId), this);
    }

    public override int Insert(bool isReturnId = false)
    {
        AddressID = Address.Insert(true);
        InfoID = Info.Insert(true);
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {
        if (Address.Update() && Info.Update())
        {
            Ip = WebUtil.GetUserIP();//紀錄最後異動的IP
            return EkiSql.ppyp.update(this);
        }
        return false;
    }

    public override bool Delete()
    {
        beEnable = false;        

        var now = DateTime.Now;
        sDate = now;
        eDate = now;

        Info.deleteImgWith(this);        

        return Update();
    }

    public string tokenRaw() => UniqueID.toString();

    public EncryptFormat format() => EncryptFormat.SHA256;
    public string newSalt() => SecurityBuilder.CreateSaltKey(5);
    public void setSalt(string salt) => SecuritySalt = salt;
    public void setCipher(string cipher) => Pwd = cipher;
    public string salt() => SecuritySalt;
    public string cipher() => Pwd;
    public string socketID() => UniqueID.toString();
    public string fcmToken() => PushToken;
    public MobilType device() => mobilType;

    public MemberResponse convertToResponse() => new MemberResponse
    {
        Phone = PhoneNum,
        NickName = Info.NickName
    };

    public string countryCode() => Info.CountryCode;

    public string phone() => PhoneNum;

}