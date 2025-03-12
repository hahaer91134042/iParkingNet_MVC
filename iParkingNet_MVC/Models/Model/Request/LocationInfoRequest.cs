using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocationInfoRequest 的摘要描述
/// </summary>
public class LocationInfoRequest : RequestAbstractModel,
                                                                IRequestConvert<LocationInfo>, 
                                                                ApiFeature_v2.Request,
                                                                ApiFeature_v2.IRequestConvert<LocationInfo>
{
    public int current { get; set; }
    public int charge { get; set; }


    public string content { get; set; }
    public int position { get; set; } = 0;
    public int size { get; set; } = 0;
    public int type { get; set; } = 0;
    public double height { get; set; } = -1;
    public double weight { get; set; } = -1;



    public bool isValid_v2()
    {
        if (!(position.containEnumValue<SitePosition>() && size.containEnumValue<SiteSize>()))
            return false;        

        return true;
    }

    public override bool isValid()
    {
        return (current.containEnumValue<CurrencyUnit>()) && (charge.containEnumValue<ChargeSocket>())&&
            (position.containEnumValue<SitePosition>()) &&(size.containEnumValue<SiteSize>());
    }
    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(content);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public LocationInfo convertToDbModel()
    {
        return new LocationInfo()
        {
            Current=current,
            Charge=charge,
            InfoContent=content,
            Position=position,
            Size=size,
            Type=type,
            Height=height,
            Weight=weight
        };
    }

    public LocationInfo convertToModel_v2(Action<LocationInfo> back = null)
    {
        return new LocationInfo()
        {
            Current = CurrentUnit.Abort.toInt(),
            Charge = ChargeSocket.Abort.toInt(),
            InfoContent = content,
            Position = position,
            Size = size,
            Type = type,
            Height = height,
            Weight = weight
        };
    }
}