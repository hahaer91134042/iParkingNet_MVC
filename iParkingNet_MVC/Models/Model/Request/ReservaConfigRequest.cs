using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservationRequest 的摘要描述
/// </summary>
public class ReservaConfigRequest:RequestAbstractModel,IRequestConvert<ReservaConfig>
{
    public bool beEnable { get; set; } = true;
    public bool beRepeat { get; set; } = true;
    public string text { get; set; } = "";
    public decimal price { get; set; } = 0;
    public int unit { get; set; } = 0;
    public int method { get; set; } = 0;
    public RequestObjList<OpenTimeRequest> openSet { get; set; }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(text);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool isValid()
    {
        return unit<=(int)CurrencyUnit.RMB && method<=(int)PriceMethod.Per30Min && openSet.isValid();
        // return unit<=(int)CurrencyUnit.RMB && method<=(int)PriceMethod.Per30Min && openSet.isValid();
    }

    public ReservaConfig convertToDbModel()
    {
        //new ReservaConfig().ForbiTimeList.load(forbiSet);

        return new ReservaConfig()
        {
            beEnable=beEnable,
            beRepeat=beRepeat,
            Text=text,
            Price=price,
            Unit=unit,
            Method=method,
            OpenSet = openSet.convertToDbObjList<OpenTime>()
            //OpenSet =ListConvert.convertToDbList<OpenTime,OpenTimeRequest>(openSet)
        };
    }

}