using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Card 的摘要描述
/// </summary>
[DbTableSet("OCPP_Auth")]
public class OCPP_Auth : BaseDbDAO
{
    [DbRowKey("CpSerial", DbAction.Update, false)]
    public string CpSerial { get; set; }
    [DbRowKey("Auth")]
    public string Auth { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime sDate { get; set; }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public static bool checkAuth(string cpSerial, string auth)
        => EkiSql.ppyp.hasData<OCPP_Auth>(QueryPair.New()
        .addQuery("CpSerial", cpSerial)
        .addQuery("Auth", auth)
        .addQuery("beEnable", 1));

}