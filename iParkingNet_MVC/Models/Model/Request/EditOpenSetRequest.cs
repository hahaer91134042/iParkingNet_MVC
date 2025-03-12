using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EditLocationRequest 的摘要描述
/// </summary>
public class EditOpenSetRequest:RequestAbstractModel
{

    public int id { get; set; } = 0;
    public string serNum { get; set; } = "";
    public RequestObjList<OpenTimeRequest> openSet { get; set; } = new RequestObjList<OpenTimeRequest>();
    public string time { get; set; }

    public override bool isValid()
    {
        var isDate = true;
        
        if (!time.isNullOrEmpty())
        {
            time.Trim();
            time.toDateTime(ApiConfig.DateTimeFormat, b => isDate = b);
        }
            

        return (id > 0 || !serNum.isNullOrEmpty()) && openSet.isValid()&&isDate;
    }

}