using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocSocket 拿來做成多重選項
/// </summary>
[DbTableSet("LocationSocket")]
public class LocSocket : BaseDbDAO,ILinkId
{
    [DbRowKey("LocationId", false)]
    public int LocationId { get; set; }
    [DbRowKey("Current", DbAction.Update)]
    public int Current { get { return convertEnumToInt(currentUnit); } set { currentUnit = convertIntToEnum<CurrentUnit>(value); } }
    [DbRowKey("Charge", DbAction.Update)]
    public int Charge { get { return convertEnumToInt(chargeSocket); } set { chargeSocket = convertIntToEnum<ChargeSocket>(value); } }

    public CurrentUnit currentUnit = CurrentUnit.NONE;
    public ChargeSocket chargeSocket = ChargeSocket.NONE;

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }

    public int linkId() => LocationId;
}