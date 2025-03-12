using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_OCPP;

/// <summary>
/// 紀錄Location底下的充電裝置
/// </summary>
[DbTableSet("OCPP_CP")]
public class OCPP_CP : BaseDbDAO,IConvertResponse<CpResponse>
{
    [DbRowKey("LocSerial",DbAction.Update,false)]
    public string LocSerial { get; set; }
    [DbRowKey("CpSerial",DbAction.Update,false)]
    public string CpSerial { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }


    /// <summary>
    /// 找出該CP所設置的地點
    /// 這邊不能用property會形成無窮迴圈
    /// </summary>
    public Location location()
    {
        if (loc == null)
            loc = new Location().Also(l => l.CreatBySerNum(LocSerial));
        return loc;
    }


    /// <summary>
    /// 該CP底下所依附的認證卡片
    /// </summary>
    public List<OCPP_Auth> cpAuth 
    { 
        get
        {
            if (auths == null)
            {
                auths = (from c in EkiSql.ppyp.table<OCPP_Auth>()
                         where c.beEnable
                         where c.CpSerial == CpSerial
                         where c.Auth != EkiOCPP.Config.EkiAdminIdTag
                         select c).toSafeList();
            }
            return auths;
        } 
    }

    private List<OCPP_Auth> auths;
    private Location loc;

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);


    public static OCPP_CP CreatByCpSerial(string cpSerial)
        => (from c in EkiSql.ppyp.table<OCPP_CP>()
            where c.beEnable
            where c.CpSerial == cpSerial
            select c).FirstOrDefault();
    public static bool checkCP(string serial)
        => EkiSql.ppyp.hasData<OCPP_CP>(QueryPair.New()
                .addQuery("CpSerial", serial)
                .addQuery("beEnable", 1));

    public CpResponse convertToResponse()
        => new CpResponse()
        {
            Serial=CpSerial
        };
}