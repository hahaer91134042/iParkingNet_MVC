using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// VehicleInfo 的摘要描述
/// </summary>
[DbTableSet("VehicleInfo")]
public class VehicleInfo : BaseDbDAO,IConvertResponse<VehicleInfoResponse>,IMapImg
{
    /**
     *  [Id]
      ,[MemberId]
      ,[Manufacturer]
      ,[Label]
      ,[Type]
      ,[Img]
      ,[UniqueID]
      ,[IsDefault]
     * */
    [DbRowKey("MemberId",false)]
    public int MemberId { get; set; }
    //[DbRowKey("Manufacturer",DbAction.Update)]//車廠
    //public string Manufacturer { get; set; }
    [DbRowKey("Label",DbAction.Update)]//品牌名稱
    public string Label { get; set; }
    [DbRowKey("Type",DbAction.Update)]//車型
    public string Type { get; set; }
    [DbRowKey("Current",DbAction.Update)]
    public int Current { get { return convertEnumToInt(currentUnit); } set { currentUnit = convertIntToEnum<CurrentUnit>(value); } }
    [DbRowKey("Charge",DbAction.Update)]
    public int Charge { get { return convertEnumToInt(chargeSocket); } set { chargeSocket = convertIntToEnum<ChargeSocket>(value); } }
    [DbRowKey("Number",DbAction.Update)]
    public string Number { get; set; }
    [DbRowKey("Name",DbAction.Update)]
    public string Name { get; set; }
    [DbRowKey("Img",DbAction.Update)]
    public string Img { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("IsDefault",DbAction.Update)]
    public bool IsDefault { get; set; }

    public CurrentUnit currentUnit = CurrentUnit.NONE;
    public ChargeSocket chargeSocket = ChargeSocket.NONE;

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }
    public override bool CreatByUniqueId(string uniqueId)
    {
        return EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("UniqueID", uniqueId), this);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }
    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }

    public VehicleInfo mapImgPath(string memberUniqueID)
    {
        if(!string.IsNullOrEmpty(Img))
            Img = $"{WebUtil.getWebURL()}{DirPath.Member}/{memberUniqueID}/{Img}";
        return this;
    }

    public void UpdateValue(VehicleRequest request)
    {
        Label = request.label;
        Type = request.type;
        Current = request.current;
        Charge = request.charge;
        Number = request.number;
        Name = request.name;
        IsDefault = request.isDefault;
    }

    public VehicleInfoResponse convertToResponse()
    {
        return new VehicleInfoResponse()
        {
            Label=Label,
            Type=Type,
            Current=Current,
            Charge=Charge,
            Number=Number,
            Img=Img,
            Name=Name,
            Id=Id,
            IsDefault=IsDefault
        };
    }

    public string imgName() => Img;
}