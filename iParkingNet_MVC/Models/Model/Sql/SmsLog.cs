using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SmsLog 的摘要描述
/// </summary>
[DbTableSet("SmsLog")]
public class SmsLog : BaseDbDAO
{
    [DbRowKey("Lan")]
    public string Lan { get { return lanEnum.ToString(); } set { lanEnum = value.toEnum<LanguageFamily>(); } }
    [DbRowKey("CountryCode")]
    public string CountryCode { get; set; }
    [DbRowKey("Phone")]
    public string Phone { get; set; }
    [DbRowKey("Code")]
    public string Code { get; set; }
    [DbRowKey("Text")]
    public string Text { get; set; }
    [DbRowKey("Msgid")]
    public int Msgid { get; set; }
    [DbRowKey("Descript")]
    public string Descript { get; set; }
    [DbRowKey("Ip")]
    public string Ip { get; set; }
    [DbRowKey("cDate",RowAttribute.CreatTime,true)]
    public DateTime cDate { get; set; }


    public LanguageFamily lanEnum;

    SmsLog() { }

    public static SmsLog creat(IPhoneMap map) => new SmsLog
    {
        CountryCode = map.countryCode(),
        Phone = map.phone()
    };


    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
}