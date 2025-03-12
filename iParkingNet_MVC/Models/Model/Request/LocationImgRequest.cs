using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocationImgRequest 的摘要描述
/// </summary>
public class LocationImgRequest:RequestAbstractModel,IRequestConvert<LocImg>
{
    public string serNum { get; set; } = "";
    public int sort { get; set; } = 0;

    public override bool isValid()
    {   
        return SerialNumUtil.isLocationSerialNum(serNum);
    }

    public LocImg convertToDbModel() =>
        new LocImg
        {
            Sort = sort
        };
}