using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// LocationInfo 的摘要描述
/// </summary>
[DbTableSet("LocationInfo")]
public class LocationInfo : BaseDbDAO,IEdit<LocationInfoRequest>
{
    [DbRowKey("Current", DbAction.Update)]
    public int Current { get { return convertEnumToInt(currentUnit); } set { currentUnit = convertIntToEnum<CurrentUnit>(value); } }
    [DbRowKey("Charge", DbAction.Update)]
    public int Charge { get { return convertEnumToInt(chargeSocket); } set { chargeSocket = convertIntToEnum<ChargeSocket>(value); } }
    [DbRowKey("Status",DbAction.Update)]
    public int Status { get; set; }
    [DbRowKey("SerialNumber",false)]
    public string SerialNumber { get; set; }
    [DbRowKey("InfoContent",DbAction.Update)]
    public string InfoContent { get; set; }
    [DbRowKey("Img", DbAction.Update)]
    public string Img { get; set; }
    [DbRowKey("Position", DbAction.Update)]
    public int Position { get { return sitePosition.toInt(); } set { sitePosition = value.toEnum<SitePosition>(); } }
    [DbRowKey("Size", DbAction.Update)]
    public int Size { get { return siteSize.toInt(); } set { siteSize = value.toEnum<SiteSize>(); } }
    [DbRowKey("Type",DbAction.Update)]
    public int Type { get { return siteType.toInt(); } set { siteType = value.toEnum<SiteType>(); } }
    [DbRowKey("Height",DbAction.Update)]
    public double Height { get; set; }
    [DbRowKey("Weight",DbAction.Update)]
    public double Weight { get; set; }


    public CurrentUnit currentUnit = CurrentUnit.NONE;
    public ChargeSocket chargeSocket = ChargeSocket.NONE;
    public SitePosition sitePosition = SitePosition.OutDoor;
    public SiteSize siteSize = SiteSize.Standar;
    public SiteType siteType = SiteType.Flat;


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
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }

    //public string imgName() => Img;

    public void editBy(LocationInfoRequest data,int ver=1)
    {
        switch (ver)
        {
            case 2:
                Current = CurrentUnit.Abort.toInt();
                Charge = ChargeSocket.Abort.toInt();
                break;
            default:
                Current = data.current;
                Charge = data.charge;
                break;
        }
        InfoContent = data.content;
        Position = data.position;
        Size = data.size;
        Type = data.type;
        Height = data.height;
        Weight = data.weight;
    }
}